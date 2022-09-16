using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CombinationsItem
{
	[JsonProperty("elements")]
	public List<int> elements { get; set; }

	[JsonProperty("product")]
	public int product { get; set; }
}

[Serializable]
public class ElementsItem
{
	[JsonProperty("id")]
	public int id { get; set; }

	[JsonProperty("name")]
	public string name { get; set; }
}

[Serializable]
public class LevelsItem
{
	[JsonProperty("name")]
	public string name { get; set; }

	[JsonProperty("id")]
	public int id { get; set; }

	[JsonProperty("goal")]
	public int goal { get; set; }


	[JsonProperty("elements")]
	public List<ElementsItem> elements { get; set; }

	[JsonProperty("combinations")]
	public List<CombinationsItem> combinations { get; set; }
}

[Serializable]
public class GameRules
{
	[JsonProperty("game")]
	public string game { get; set; }

	[JsonProperty("members")]
	public List<string> members { get; set; }

	[JsonProperty("levels")]
	public List<LevelsItem> levels { get; set; }
}

/**
 * Read Resources/combinations.json file and handle the possible combination
 */
public class CombinationManager : MonoBehaviour
{
	private GameRules rules;

	/*
	 * Initialize the GameRules Object
	 * Using JsonParser to parse the /Resources/meta.json file to get the object
	 */
	private void Start()
    {
		// Ref https://docs.unity3d.com/ScriptReference/Resources.Load.html
		TextAsset targetFile = Resources.Load<TextAsset>("combination");

		if (targetFile == null)
		{
			Debug.LogError("No such File!");
		} else {
			string json = targetFile.text;
			Debug.Log(json);
			rules = JsonConvert.DeserializeObject<GameRules>(json);
			Debug.Log("COMBINATIONS RULES LOAD SUCCESSFULLY!");
		}

	}

	/**
	 * return element id by name, return -1 when doesn't contain
	 * @param name Name of the object
	 * @param level Level that the player currently on
	 */
	public int getIdByElementName(string name, int level)
	{
		List<ElementsItem> levelElements = rules.levels[level].elements;
		foreach (ElementsItem e in levelElements)
        {
			if (e.name.ToLower() == name.ToLower())
			{
				return e.id;
			}
		}
		return -1;
	}

	/**
	 * return id by name, return "NotFound" when doesn't contain
	 * @param id ID of the gameobject
	 * @param level Level that the player currently on
	 */
	public string getElementNameById(int id, int level)
	{
		List<ElementsItem> levelElements = rules.levels[level].elements;
		foreach (ElementsItem e in levelElements)
		{
			if (e.id == id)
			{
				return e.name;
			}
		}
		return "NotFound";
	}

	/**
	 * get the final goal of a level
	 * @param level Level that the player currently on
	 */
	public ElementsItem getLevelGoalsElement(int level)
	{
		int goalId = rules.levels[level].goal;

		ElementsItem e = new ElementsItem();

		string elementName = getElementNameById(goalId, level);
		if (elementName.IndexOf("NotFound") == -1)
		{
			e.name = elementName;
			e.id = getIdByElementName(elementName, level);
		}
		

		return e;
	}

	/**
	 * get All elements that are able to be combined with the given element
	 * @param elementName Element that should be combined
	 * @param level Level that the player currently on
	 */
	public List<ElementsItem> getCombinableElementsByElementName(string elementName, int level)
    {
		List<CombinationsItem> levelCombinations = rules.levels[level].combinations;
		List<ElementsItem> possibleCombinationItems = new List<ElementsItem>();
		int elementId = getIdByElementName(elementName, level);

        foreach (CombinationsItem comb in levelCombinations)
		{
			int itemA = comb.elements[0];
			int itemB = comb.elements[1];
			if (itemA == elementId)
            {
				ElementsItem e = new ElementsItem();
				e.name = getElementNameById(itemB, level);
				e.id = itemB;
				possibleCombinationItems.Add(e);
            } else if (itemB == elementId)
            {
				ElementsItem e = new ElementsItem();
				e.name = getElementNameById(itemA, level);
				e.id = itemA;
				possibleCombinationItems.Add(e);
			}
		}
		return possibleCombinationItems;
    }

	/**
	 * check whether the two elements are able to be combined
	 * @param elementA Element A that the merge behaviour should be checked
	 * @param elementB Element B that the merge behaviour should be checked
	 * @param level Level that the player currently on
	 */
	public bool checkCompatibility(string elementA, string elementB, int level)
    {
        List<ElementsItem> possibleCombinationItems = getCombinableElementsByElementName(elementA, level);

		foreach (ElementsItem possibleItem in possibleCombinationItems)
        {
			if (possibleItem.name.ToLower().IndexOf(elementB.ToLower()) != -1)
            {
				return true;
            }
        }
		return false;
	}

	/**
	 * get the combination result of two elements
	 * @param elementNameA Element A that should be combined
	 * @param elementNameB Element B that should be combined
	 * @param level Level that the player currently on
	 * @return the combination result
	 * 
	 */
	public ElementsItem getProductElementOfCombination(string elementNameA, string elementNameB, int level)
	{
		int elementIdA = getIdByElementName(elementNameA, level);
		int elementIdB = getIdByElementName(elementNameB, level);
		if (checkCompatibility(elementNameA, elementNameB, level)) {
			List<CombinationsItem> levelCombinations = rules.levels[level].combinations;
			foreach (CombinationsItem comb in levelCombinations)
			{
				int itemA = comb.elements[0];
				int itemB = comb.elements[1];

				if ((itemA == elementIdA && itemB == elementIdB) || (itemA == elementIdB && itemB == elementIdA))
				{
					ElementsItem e = new ElementsItem();
					e.name = getElementNameById(comb.product, level);
					e.id = comb.product;
					return e;
				}
			}
		}

		return null;
	}

	/**
	 * just a test function
	 */
	public void test()
    {
		Debug.Log("Get ElementName Of 101:");
		Debug.Log(getElementNameById(101, 0));
		Debug.Log("Get ElementName Of -1 (Wrong Case):");
		Debug.Log(getElementNameById(-1, 0));

		Debug.Log("Get Id Of Cactus:");
		Debug.Log(getIdByElementName("Cactus", 0));
		Debug.Log("Get Id Of cactus (lower case should work):");
		Debug.Log(getIdByElementName("cactus", 0));

		Debug.Log("Get Id Of Airplane (Wrong Case):");
		Debug.Log(getIdByElementName("Airplane", 0));

		ElementsItem goals = getLevelGoalsElement(0);
		Debug.Log("First Level Goal:");
		Debug.Log("Id: " + goals.id + ", Name: " + goals.name);

		List<ElementsItem> combinablesOfSoil = getCombinableElementsByElementName("Soil", 0);
		Debug.Log("Combiables Of Soil:");
		foreach (ElementsItem ef in combinablesOfSoil)
		{
			Debug.Log("Id: " + ef.id + ", Name: " + ef.name);
		}

		Debug.Log("Check Compability of Life and Earth:");
		Debug.Log(checkCompatibility("Life", "Earth", 0));
 

		Debug.Log("Check Compability of life and earth (lower case should work):");
		Debug.Log(checkCompatibility("life", "earth", 0));

		Debug.Log("Check Compability of Water and Water (Wrong case):");
		Debug.Log(checkCompatibility("Water", "Water", 0));

		Debug.Log("Check Compability of Time and ku (Wrong Case):");
		Debug.Log(checkCompatibility("Time", "ku", 0));


		Debug.Log("Get Product of Fire and Fire");
		ElementsItem e = getProductElementOfCombination("Fire", "Fire", 0);
		if (e != null)
			Debug.Log("Id: " + e.id + ", Name: " + e.name);
		else
			Debug.Log("No such Combination!");

		Debug.Log("Get Product of Animal and time (lower case should work):");
		e = getProductElementOfCombination("Animal", "time", 0);
		if (e != null)
			Debug.Log("Id: " + e.id + ", Name: " + e.name);
		else
			Debug.Log("No such Combination!");

		Debug.Log("Get Product of Cloud and Fire (Wrong Case)");
		e = getProductElementOfCombination("Cloud", "Fire", 0);
		if (e != null)
			Debug.Log("Id: " + e.id + ", Name: " + e.name);
		else
			Debug.Log("No such Combination!");

		Debug.Log("Get Product of Wind and ku (Wrong Case)");
		e = getProductElementOfCombination("Wind", "ku", 0);
		if (e != null)
			Debug.Log("Id: " + e.id + ", Name: " + e.name);
		else
			Debug.Log("No such Combination!");
	}
}
