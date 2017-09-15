using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueTable{
	// struct that contains a character name and their dialogue
	public struct Chat{
		public string name;
		public string text;
		public Chat(string n, string t){
			name = n;
			text = t;
		}
	}

	static Dictionary<string, Chat> EnDialogueMap = new Dictionary<string, Chat>();
	static Dictionary<string, Chat> JpDialogueMap = new Dictionary<string, Chat>();
	static bool loadedDialogue = false;
	
	public static string language = "eng";
	public static Dictionary<string, Dictionary<string, Chat>> LanguageOptions = new Dictionary<string, Dictionary<string, Chat>>();

	// Creates a chain of dialogue
	void AddChat(string chatName, string character, string text){
		if (LanguageOptions[language].ContainsKey(chatName)){
			AddChat(chatName + "1", character, text);
		} else {
			LanguageOptions[language][chatName] = new Chat(character, text);
		}
	}

	/// <summary>
	/// Returns a chain of dialogue that was automatically added to the dictionary
	/// </summary>
	/// <returns>The chat chain.</returns>
	/// <param name="chatName">Chat name.</param>
	public static ArrayList GetChatChain(string chatName){
		ArrayList chatChain = new ArrayList();
		string name = chatName;
		while (LanguageOptions[language].ContainsKey(name)){
			chatChain.Add(LanguageOptions[language][name]);
			name += "1";
		}
		return chatChain;
	}


	//public DialogueTable(){
	public static bool PrepDialogue(){
		if (loadedDialogue){
			return true;
		}
		LanguageOptions.Add("eng", EnDialogueMap);
		LanguageOptions.Add("jp", JpDialogueMap);

		EnDialogueMap["thisempire"] = new Chat("sittinggod", "THIS EMPIRE IS YOURS");
		JpDialogueMap["thisempire"] = new Chat("sittinggod", "この 帝国は お前の");

		EnDialogueMap["YOU"] = new Chat("sittinggod", "YOU");
		EnDialogueMap["ARE"] = new Chat("sittinggod", "WILL");
		EnDialogueMap["DEAD"] = new Chat("sittinggod", "DIE");

		JpDialogueMap["YOU"] = new Chat("sittinggod", "魂");
		JpDialogueMap["ARE"] = new Chat("sittinggod", "は");
		JpDialogueMap["DEAD"] = new Chat("sittinggod", "死ぬ");

		EnDialogueMap["doyouwantrevenge"] = new Chat("sittinggod", "DO YOU WANT REVENGE?");
		JpDialogueMap["doyouwantrevenge"] = new Chat("sittinggod", "復讐　が　必要　ある?");

		EnDialogueMap["onemorechance"] = new Chat("sittinggod", "WE'LL GIVE YOU ONE CHANCE");
		JpDialogueMap["onemorechance"] = new Chat("sittinggod", "一回　だけ　やらせるぞ");

		EnDialogueMap["wakeup"] = new Chat("sittinggod", "NOW WAKE UP");
		JpDialogueMap["wakeup"] = new Chat("sittinggod", "今 起きろ");

		//EnDialogueMap.Add("prisonerexposition", "Keep it on the down low, but unlock this door for me and you'll be glad you did.");
		EnDialogueMap["prisonerexposition"] = new Chat("prisoner", "Keep it on the down low, but unlock this door for me and you'll be glad you did.");
		// Meeting first boss
		/*
		EnDialogueMap["firstbossexposition"] = "I see you followed me. The identity you saw was Danilo Ilic--my true form was just a little too big for that old room, huh?";
		EnDialogueMap["firstbossexposition1"] = "As thanks, you're welcome to join our legion and help unite the powers once more.";
		EnDialogueMap["firstbossexposition2"] = "No. This world's to be devoured. Inside me, all is one.";
		*/

		EnDialogueMap["firstbossexposition"] = new Chat("Gavrilo Princip", "We meet again.");
		EnDialogueMap["firstbossexposition1"] = new Chat("Gavrilo Princip", "As thanks for helping me get out of that pen, I'd like to offer an invitation to our secret society.");
		EnDialogueMap["firstbossexposition11"] = new Chat("Gavrilo Princip", "You see, we're hoping to unite these creatures under our guidance, and, well...");
		EnDialogueMap["firstbossexposition111"] = new Chat("Gavrilo Princip", "How would you like to join The Black Hand?");
		EnDialogueMap["firstbossexposition1111"] = new Chat("hand", "No. This world's to be devoured. Inside me, all is one.");
		EnDialogueMap["firstbossexposition11111"] = new Chat("Gavrilo Princip", "Well then! Let me show you what I think of tyrannical consumerism.");

		// Hand/Bullet Boss tells player that he can't be damaged
		EnDialogueMap["firstbossweakness"] = new Chat("Gavrilo Princip", "YOU MUST REALIZE THAT YOU ALONE CAN'T DEFEAT THE STRONG. YOU WIN BY LEADING THE STRONGER.");

		// When first encountering Franz
		EnDialogueMap["franzfirst"] = new Chat("hand", "Suddenly, I feel I owe that old criminal a favor.");
		EnDialogueMap["franzfirst1"] = new Chat("hand", "I should recall his Memory and let him do the job through his own hands.");

		// After Franz is destroyed
		EnDialogueMap["franzdead"] = new Chat("hand", "With this shot, darkness will fall upon the earth.");
		EnDialogueMap["franzdead1"] = new Chat("hand", "With this shot, these animals will hold my hand for guidance and depend on my eye to see.");
		EnDialogueMap["franzdead11"] = new Chat("hand", "May I reign over a global empire once more, and this time, forever.");

		// Hand/Bullet Boss tells player that he can't be damaged
		EnDialogueMap["bulletbosseat"] = new Chat("Gavrilo Princip", "DEVOUR me. Through me, you can BECOME Them.");

		// Hand/Bullet Boss tells player that he can't be damaged
		EnDialogueMap["firstwatchingspirit"] = new Chat("Spirit", "Seems that you think you can take this earth as your own.");
		EnDialogueMap["firstwatchingspirit1"] = new Chat("Spirit", "You may very well do so, but the heavens are our domain.");
		EnDialogueMap["firstwatchingspirit11"] = new Chat("Spirit", "Never forget a planet's place in the universe.");
		// Dialogue for after the player is crushed by the spirit
		EnDialogueMap["firstwatchingspirit2"] = new Chat("Spirit", "...But...");
		EnDialogueMap["firstwatchingspirit21"] = new Chat("Spirit", "...Don't stop trying to reach higher.");

		// Dialogue for when the Water Spirit appears
		EnDialogueMap["waterspiritappear"] = new Chat("WaterSpirit", "WATER FLOWS THROUGH ALL LIFE");
		EnDialogueMap["waterspiritappear2"] = new Chat("WaterSpirit", "AND IN DEATH IT IS FREED");
		EnDialogueMap["waterspiritescape"] = new Chat("WaterSpirit", "FOLLOW ME INTO HELL");
		EnDialogueMap["waterspiritdie"] = new Chat("WaterSpirit", "MAY THE WATER THAT LEAVES ME BE A NEW BEGINNING");
		EnDialogueMap["waterspiritdoit"] = new Chat("WaterSpirit", "CONSUME ME");

		// Dialogue for escaping after getting the folder
		EnDialogueMap["gotfirstfolder"] = new Chat("TheHand", "THEY ARE COMING SO RUN");

		loadedDialogue = true;
		return loadedDialogue;
	}

	// Returns the selected dialogue
	public static string GetText(string index){
		Dictionary<string, Chat> selectedDictionary = LanguageOptions[language];
		return selectedDictionary[index].text;
	}

	// Returns the selected dialogue
	public static Chat GetChat(string index){
		Dictionary<string, Chat> selectedDictionary = LanguageOptions[language];
		return selectedDictionary[index];
	}

	public static string LowRank(){
		return "Fall back. Your rank's too low to enter.";
	}
	
}
