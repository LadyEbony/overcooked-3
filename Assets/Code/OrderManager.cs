using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderManager : EntityBase, EntityNetwork.IMasterOwnsUnclaimed {

  public static OrderManager Instance { get; private set; }

  [Header("Game")]
  public bool ready;
  public List<Recipe> orders;

  public static int score;
  private int prevScore;

  public int minOrders = 2;
  public float orderTimer = 12f;
  private float nextOrder;

  [Header("UI")]
  public Transform uiTransform;
  public TextMeshProUGUI scoreTextMesh;
  private GameObject uiPrefab;
  public Sprite cutSprite, cookSprite;

  [Header("Sound")]
  public AudioSource source;
  public AudioClip success;

  [Header("Network")]
  public float networkTimer = 0.1f;
  private float nextNetworkTimer;

  public override void Awake() {
    base.Awake();
    Instance = this;
  }

  IEnumerator Start() {
    uiPrefab = uiTransform.GetChild(0).gameObject;
    uiPrefab.SetActive(false);
    uiPrefab.transform.parent = null;

    EntityID = -10;
    authorityID = -1;
    Register();

    orders = new List<Recipe>();
    score = 0;

    while (!NetworkManager.gameReady) yield return null;
    ready = true;
  }

  // Update is called once per frame
  void Update() {
    if (!ready) return;

    // add recipes (gameplay loop)
    if (isMine){
      // always two orders
      if (orders.Count < minOrders){
        CreateOrder();
      }

      // you have to make some orders
      else if (Time.time >= nextOrder){
        CreateOrder();
      }
    }

    // display to ui
    // first create ui
    while(uiTransform.childCount < orders.Count){
      var item = Instantiate(uiPrefab, uiTransform);
      item.SetActive(true);
    }

    while(uiTransform.childCount > orders.Count){
      var item = uiTransform.GetChild(0);
      item.parent = null;
      Destroy(item.gameObject);
    }

    // then set
    for(var i = 0; i < orders.Count; ++i){
      var order = orders[i];
      var ui = uiTransform.GetChild(i);

      ui.Find("Order").GetComponent<Image>().sprite = order.sprite;
      ui.Find("Text").GetComponent<TextMeshProUGUI>().text = order.name;

      // copy from prefab (assumes that there is always one ingredient)
      var ingredientsTransform = ui.Find("Ingredients");
      while(ingredientsTransform.childCount < order.ingredients.Length){
        Instantiate(ingredientsTransform.GetChild(0).gameObject, ingredientsTransform);
      }

      for(var j = 0; j < order.ingredients.Length; j++){
        var ing = ItemContainer.Instance.ingredients[order.ingredients[j]];
        var ui2 = ingredientsTransform.GetChild(j);

        var item = ui2.Find("Item").GetComponent<Image>();
        var type = ui2.Find("Type").GetComponent<Image>();
        item.sprite = ing.sprite;

        var food = ing.prefab.GetComponent<ItemDescription>();
        if (food.cutAmount > 0){
          type.gameObject.SetActive(true);
          type.sprite = cutSprite;
        } else if (food.cookAmount > 0){
          type.gameObject.SetActive(true);
          type.sprite = cookSprite;
        } else {
          type.gameObject.SetActive(false);
        }
      }
    }

    scoreTextMesh.text = score.ToString();

    if (prevScore != score){
      source.PlayOneShot(success);
    }
    prevScore = score;

    // network recipes
    if (NetworkManager.inRoom && isMine && Time.time >= nextNetworkTimer){
      UpdateNow();
      nextNetworkTimer = Time.time + networkTimer;
    }
  }

  void CreateOrder(){
    orders.Add(ItemContainer.Instance.GetRandomRecipe);
    nextOrder = Time.time + orderTimer;
  }

  public bool IsAnOrder(int recipeIndex){
    foreach(var o in orders){
      if (o.index == recipeIndex) return true;
    }
    return false;
  }

  public void RemoveOrder(int recipeIndex){
    for(var i = 0; i < orders.Count; ++i){
      if (orders[i].index == recipeIndex){
        orders.RemoveAt(i);

        score += 1;
        return;
      }
    }
  }

  public override void Serialize(ExitGames.Client.Photon.Hashtable h) {
    base.Serialize(h);

    h.Add('o', orders.Select(o => o.index).ToArray());
    h.Add('s', score);
  }

  public override void Deserialize(ExitGames.Client.Photon.Hashtable h) {
    base.Deserialize(h);

    orders = (h['o'] as int[]).Select(o => ItemContainer.Instance.recipes[o]).ToList();
    score = (int)h['s'];
  }

}
