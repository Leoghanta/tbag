using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TBAG
{
	internal class Program
	{

		/*   Create a Maze
		 *   
		 *   Movement Commands - North, East, West, South
		 *  Each room is a dictionary of Title, Description and directions.
		 *  The rooms are stored in a larger dictionary called ROOMS
		 */

		static Dictionary<string, Dictionary<string, object>> ROOMS = new Dictionary<string, Dictionary<string, object>>
		{
			//Start
			{"start", new Dictionary<string, object>
				{
					{"title", "The Start of the Maze" },
					{"desc", "You are at the start of a large maze" },
					{"items", new List<string>{"parachute" } },
					{"east", "maze" },
				}
			},

			//ROOM1
			{"maze", new Dictionary<string, object>
				{
					{"title", "Lost in the Maze" },
					{"desc", "Ah crap! You're lost in the maze!" },
					{"items", new List<string>{"apple" } },
					{"west", "start" },
					{"east", "cliff" }
				}
			},

			//ROOM2
			{"cliff", new Dictionary<string, object>
				{
					{"title", "Top of the cliff" },
					{"desc", "You are on the top of a cliff looking out to see.!"},
					{"west", "maze" },
				}
			},

			//ROOM3
			{"beach", new Dictionary<string, object>
				{
					{"title", "Beach by the cliff" },
					{"desc", "You are on a small beach at the bottom of a cliff.!"},
					{"south", "cave" },
					{"items", new List<string>{"sword"} }
				}
			},
			//ROOM4
			{"cave", new Dictionary<string, object>
				{
					{"title", "In a cave by the beach" },
					{"desc", "You are in a dank cave. The only exit is north back to the beach."},
					{"north", "beach" },
					{"npc", new List<string>{"troll"} }
				}
			},
		};


		// ITEMS
		static Dictionary<string, Dictionary<string, object>> ITEMS = new Dictionary<string, Dictionary<string, object>>
		{
			//Start
			{"parachute", new Dictionary<string, object>
				{
					{"title", "A small parachute in a backpack" },
					{"desc", "It's well packed and ready to deploy" },
				}
			},

			{"apple", new Dictionary<string, object>
				{
					{"title", "A red juicy apple" },
					{"desc", "It's probably not poisonous" },
					{"food", 10 },
				}
			},

			{"sword", new Dictionary<string, object>
				{
					{"title", "The broadsword of impecable power" },
					{"desc", "It seems to vibrate with power." },
					{"damage", 25 },
				}
			},

			{"club", new Dictionary<string, object>
				{
					{"title", "A mighty wooden club" },
					{"desc", "Best used for hitting people with" },
					{"damage", 10 },
				}
			},
			{"horn", new Dictionary<string, object>
				{
					{"title", "A beautfully crafted gold horn" },
					{"desc", "Pucker up and blow!" },
				}
			}
		};

		// ITEMS
		static Dictionary<string, Dictionary<string, object>> NPC = new Dictionary<string, Dictionary<string, object>>
		{
			//Start
			{"troll", new Dictionary<string, object>
				{
					{"name", "The Cave Troll" },
					{"desc", "He looks at you cautiously." },
					{"health", 100 },
					{"damage", 10 },
					{"armour", 10 },
					{"weapon", "club" },
					{"items", new List<string>{"club", "horn"} }
				}
			},
		};


		static List<string> COMMANDS = new List<string>
		{
			"north", "south", "east", "west", "blow", "examine", "ex", "drop", "eat", "fight", "get", 
			"inventory", "inv", "jump", "kill",  "look",
		};


		static void BlowCommand(string item, List<string> inventory, string location)
		{
			//Does the item exist?
			if (!ITEMS.ContainsKey(item))
			{
				Console.WriteLine($"The {item} does not exist.");
				return;
			}
			if (!inventory.Contains(item))
			{
				Console.WriteLine($"You aren't carrying the {item}.");
				return;
			}

			// ok, let's blow it.
			if (item == "horn" && location == "beach")
			{
                Console.WriteLine("You blow the horn as hard as you can.");
                Console.WriteLine("A passing ship hears your cry and rescues you");
                Console.WriteLine("You have escaped! Congratulations");
				Environment.Exit(0);
				return;
            }
			
			if (item == "horn")
			{
				Console.WriteLine("You blow your horn.  Toot Toot!");
				Console.WriteLine("nothing happened!");
				return;
			}

            Console.WriteLine($"You put your {item} to your lips and blow");
			Console.WriteLine("That was fun, wasn't it?");
        }

		static void ExamineCommand(string item, string location, List<string> inventory)
		{
			// does the thing exist?
			if (!ITEMS.ContainsKey(item) && !NPC.ContainsKey(item))
			{
				Console.WriteLine($"{item} does not exist in this universe!");
				return;
			}

			// If they're an NPC, they need to be in the same room.
			if (NPC.ContainsKey(item))
			{
				//Is there anybody in the room with the player?
				if (ROOMS[location].ContainsKey("npc"))
				{
					//There is someone here - is it who we're looking for?
					List<string> npcList = (List<string>)ROOMS[location]["npc"];
					if (npcList.Contains(item))
					{
						//The NPC is here, examine them.
						Console.WriteLine($"You examine {NPC[item]["name"]} closely.");
						Console.WriteLine(NPC[item]["desc"]);
						return;
					}
				}
			}
			
			//Ok, it's not an NPC, let's see if it's an item.
			if (ITEMS.ContainsKey(item))
			{
				//Are there items in the room?
				if (ROOMS[location].ContainsKey("items"))
				{
					//There are items here - is it what we're looking for?
					List<string> itemList = (List<string>)ROOMS[location]["items"];
					if (itemList.Contains(item))
					{
						//The item is here, examine it.
						Console.WriteLine($"You examine the {item} closely.");
						Console.WriteLine(ITEMS[item]["desc"]);
						return;
					}
				}

				//It's not in the room, is it in my inventory?
				if (inventory.Contains(item))
				{
					Console.WriteLine("You rummage through your belongings.");
					Console.WriteLine($"You find the {item} and examine it closely.");
					Console.WriteLine(ITEMS[item]["desc"]);
					return;
				}
			}

			//Can't find the item anywhere!
			Console.WriteLine($"You look around but can't find the {item}.");
		}
	
		/// <summary>
		/// Player has died. Tell them and kick them off
		/// the game.
		/// </summary>
		/// <param name="reason">Reason for death as string</param>
		static void Death(string reason)
		{
			Console.WriteLine(new string('=', 80));
			Console.WriteLine("\tYou Have Died!");
			Console.WriteLine($"\t\t{reason}");
			Console.WriteLine(new string('=', 80));
			Environment.Exit(0);
		}

		/// <summary>
		/// Display a room on the screen.
		/// </summary>
		/// <param name="location">The current location of player</param>
		static void DisplayRoom(string location)
		{
			Console.WriteLine(ROOMS[location]["title"]);
			Console.WriteLine(ROOMS[location]["desc"]);

			// Items
			if (ROOMS[location].ContainsKey("items") || ROOMS[location].ContainsKey("npc"))
			{
				Console.WriteLine("You See:");
				if (ROOMS[location].ContainsKey("npc"))
				{
					foreach (string npc in (List<string>)ROOMS[location]["npc"])
					{
						Console.Write($"\t{NPC[npc]["name"]}");
						if ((int)NPC[npc]["health"] <= 0)
						{
							Console.WriteLine(" is dead here.");
						} else
						{
							Console.WriteLine(" is standing here.");
						}
					}
				}
				if (ROOMS[location].ContainsKey("items"))
				{
					foreach (string item in (List<string>)ROOMS[location]["items"])
					{
						Console.WriteLine($"\t{ITEMS[item]["title"]}");
					}
				}
			}

			// Exits
			Console.WriteLine("Obvious Exists are:");
			if (ROOMS[location].ContainsKey("north"))
			{
				Console.WriteLine($"\tnorth: {ROOMS[location]["north"]}");
			}
			if (ROOMS[location].ContainsKey("east"))
			{
				Console.WriteLine($"\teast:  {ROOMS[location]["east"]}");
			}
			if (ROOMS[location].ContainsKey("south"))
			{
				Console.WriteLine($"\tsouth: {ROOMS[location]["south"]}");
			}
			if (ROOMS[location].ContainsKey("west"))
			{
				Console.WriteLine($"\twest:  {ROOMS[location]["west"]}");
			}
		}


		/// <summary>
		/// Drop an item from inventory into location. 
		/// </summary>
		/// <param name="item">item to drop</param>
		/// <param name="location">player's current location</param>
		/// <param name="inventory">players inventory as list</param>
		static void DropCommand(string item, string location, List<string> inventory)
		{
			//Does the item exist?
			if (!ITEMS.ContainsKey(item))
			{
				Console.WriteLine($"The {item} does not exist.");
				return;
			}
			if (!inventory.Contains(item))
			{
				Console.WriteLine($"You aren't carrying the {item}.");
				return;
			}

			// Ok, drop the item them.
			Console.WriteLine($"You drop your {item}.");
			inventory.Remove(item);

			// Is there an items list in the room location? if not, create one.
			if (ROOMS[location].ContainsKey("items"))
			{
				List<string> roomitems = (List<string>)ROOMS[location]["items"];
				roomitems.Add(item);
			}
			else
			{
				ROOMS[location]["items"] = new List<string> { item };
			}
		}


		/// <summary>
		/// Player eats an object.. or tries to.
		/// </summary>
		/// <param name="item">item they want to eat</param>
		/// <param name="inv">inventory.</param>
		static void EatCommand(string item, List<string> inv)
		{
			if (!ITEMS.ContainsKey(item))
			{
				Console.WriteLine($"The {item} does not exist.");
				return;
			}
			if (!inv.Contains(item))
			{
				Console.WriteLine($"You aren't carrying the {item}");
				return;
			}
			if (!ITEMS[item].ContainsKey("food"))
			{
				Console.WriteLine($"You chew on the {item} but it is not tasty");
				return;
			}

			// item is here and we can eat. do eat.
			Console.WriteLine($"You eat the {item}.");
			inv.Remove(item);

			//Any Effects?
			if (item == "apple")
			{
				Death("Poisoned by an apple!");
			}
		}


		static void FightCommand(string location, string opponent, int health, List<string> inv, int damage, int armour)
		{
			//Do they exist?
			if (!NPC.ContainsKey(opponent))
			{
				Console.WriteLine($"You look around for {opponent} to fight!");
				return;
			}

			//Is there anyone in the room?
			if (!ROOMS[location].ContainsKey("npc"))
			{
				Console.WriteLine("There is nobody here to fight!");
				return;
			}

			List<string> npcList = (List<string>)ROOMS[location]["npc"];
			if (!npcList.Contains(opponent))
			{
				Console.WriteLine($"{opponent} is not here to fight.");
				return;
			}

			// OK, they're here. Are they alive?
			if ((int)NPC[opponent]["health"] <= 0)
			{
				Console.WriteLine($"You pummel the corpse of {opponent} for a bit.");
				return;
			}

			//Start the fight loop. do they have the sword?
			if (inv.Contains("sword"))
			{
				Console.WriteLine("You ready your sword for battle");
				damage += (int)ITEMS["sword"]["damage"];
			}
			else
			{
				Console.WriteLine("You ready yourself to fight.");
			}

			while (health > 0 && (int)NPC[opponent]["health"] > 0)
			{
				Console.WriteLine($"Player: {health} || {NPC[opponent]["name"]}: {NPC[opponent]["health"]}");
				//Player Strikes First
				if (inv.Contains("sword")){
					Console.WriteLine($"You swing your {ITEMS["sword"]["title"]} and hit {NPC[opponent]["name"]}");
				} else
				{
					Console.WriteLine($"You lightly jab {NPC[opponent]["name"]} with your fist.");
				}
				// Calculate Damage
				Random random = new Random();
				int hitpoint = random.Next(0, damage);
				int oppHealth = (int)NPC[opponent]["health"];
				NPC[opponent]["health"] = oppHealth - hitpoint;

				//Opponent Strikes Now.
				if ((int)NPC[opponent]["health"] > 0)
				{
					int npcDamage = (int)NPC[opponent]["damage"];
					Console.WriteLine($"{NPC[opponent]["name"]} hits you with a skillful blow!");
					health = health - random.Next(npcDamage);
				}

				Thread.Sleep(800); //Slow down the fight a bit.
            }

			//Fight has ended. who died?
			if (health <= 0){
				Death($"Slain by {NPC[opponent]["name"]}");
			} else
			{
				// drop the trolls inventory.
				if (NPC[opponent].ContainsKey("items"))
				{
					List<string> opponentInventory = (List<string>)NPC[opponent]["items"];
					foreach (string item in opponentInventory)
					{
						//if there's not a list, create one.
						if (!ROOMS[location].ContainsKey("items"))
						{
							ROOMS[location]["items"] = new List<string> { item };
						} else
						{
							List<string> roomitems = (List<string>)ROOMS[location]["items"];
							roomitems.Add(item);
						}
						Console.WriteLine($"{NPC[opponent]["name"]} drops his {item}");
					}
				}
				Console.WriteLine($"You have slain {NPC[opponent]["name"]}");
			}
		}

		/// <summary>
		/// Get Command. user picks an item up from room and puts it in inventory.
		/// </summary>
		/// <param name="item">item to get</param>
		/// <param name="location">players current location</param>
		/// <param name="inventory">player inventory</param>
		static void GetCommand(string item, string location, List<string> inventory)
		{
			//Does the item exist?
			if (!ITEMS.ContainsKey(item))
			{
				Console.WriteLine($"The {item} does not exist.");
				return;
			}
			//Are there items in the room to get?
			if (!ROOMS[location].ContainsKey("items"))
			{
				Console.WriteLine($"You fumble about but there is no {item} to get");
				return;
			}
			//is THIS item in the room?
			List<string> roomstuff = (List<string>)ROOMS[location]["items"];
			if (!roomstuff.Contains(item))
			{
				Console.WriteLine($"The {item} is not here.");
				return;
			}

			//Item exists and is here. get it.
			Console.WriteLine($"You pick up the {item}");
			roomstuff.Remove(item);
			inventory.Add(item);
		}

		/// <summary>
		/// Obtain text input from the user. 
		/// Split this into a command list
		/// Check if the command matches list of valid inputs.
		/// </summary>
		/// <returns>List of commands</returns>
		static string[] GetPlayerInput()
		{
			while (true)
			{
				Console.Write(">> ");
				string input = Console.ReadLine().ToLower();
				string[] inputsplit = input.Split(" ");

				if (COMMANDS.Contains(inputsplit[0]))
				{
					return inputsplit;
				}
				else
				{
					Console.WriteLine("I don't know what that means!");
				}
			}
		}


		/// <summary>
		/// Display player inventory
		/// </summary>
		/// <param name="inv">inventory as a list</param>
		static void InventoryCommand(List<string> inv)
		{
			if (inv.Count() < 1)
			{
				Console.WriteLine("You have nothing of note.");
			}
			else
			{
				Console.WriteLine("You rummage through your belongings");
				foreach (string item in inv)
				{
					Console.WriteLine($"\t{item}");
				}
			}
		}


		static string JumpCommand(string location, List<string> inventory)
		{
			if (location == "cliff")
			{
				if (inventory.Contains("parachute"))
				{
					Console.WriteLine("Cluching your parachute, you take a brave leap off the cliff.");
					Console.WriteLine("You float gently to the ground below.");
					return "beach";
				}
				else
				{
					Console.WriteLine("You take a brave leap off the cliff.");
					Console.WriteLine("As you fall, you make a note of your poor life choices.");
					Death(">SPLAT< Launched off a cliff!");
				}
			}
			else
			{
				Console.WriteLine("You jump around excitedly.");
			}
			return location;    //if we didn't move, return the same location.
		}

		/// <summary>
		/// Change a players location by travelling in a
		/// direction noted in the lcation's valid exits.
		/// </summary>
		/// <param name="currentlocation">players current location</param>
		/// <param name="direction">direction of travel</param>
		/// <returns>new location as string.</returns>
		static string TravelTo(string currentlocation, string direction)
		{
			if (ROOMS[currentlocation].ContainsKey(direction))
			{
				Console.WriteLine($"You travel {direction}");
				return (string)ROOMS[currentlocation][direction];
			}
			else
			{
				Console.WriteLine("You can't move in that direction");
				return currentlocation;
			}
		}


		/// <summary>
		/// Start the game loop
		/// </summary>
		static void StartGame()
		{
			string currentLocation = "start";
			List<string> inventory = new List<string> { "apple" };
			int damage = 10;
			int armour = 0;
			int health = 200;
			DisplayRoom(currentLocation);

			while (true)
			{
				string[] commandList = GetPlayerInput();
				switch (commandList[0])
				{
					case "north":
					case "east":
					case "south":
					case "west":
						currentLocation = TravelTo(currentLocation, commandList[0]);
						DisplayRoom(currentLocation);
						break;
					case "blow":
						if (commandList.Count() < 2)
						{
							Console.WriteLine("Blow what?");
						}
						else
						{
							BlowCommand(commandList[1], inventory, currentLocation);
						}
						break;
					case "examine":
					case "ex":
						if (commandList.Count() < 2)
						{
							Console.WriteLine("Examine what?");
						}
						else
						{
							ExamineCommand(commandList[1], currentLocation, inventory);
						}
						break;
					case "drop":
						if (commandList.Count() < 2)
						{
							Console.WriteLine("drop what?");
						}
						else
						{
							DropCommand(commandList[1], currentLocation, inventory);
						}
						break;
					case "eat":
						if (commandList.Count() < 2)
						{
							Console.WriteLine("eat what?");
						}
						else
						{
							EatCommand(commandList[1], inventory);
						}
						break;
					case "fight":
					case "kill":
						if (commandList.Count() < 2)
						{
							Console.WriteLine("who do you want dead?");
						}
						else
						{
							FightCommand(currentLocation, commandList[1], health, inventory, damage, armour);
						}
						break;
					case "get":
						if (commandList.Count() < 2)
						{
							Console.WriteLine("Get What?");
						}
						else
						{
							GetCommand(commandList[1], currentLocation, inventory);
						}
						break;
					case "inventory":
					case "inv":
						InventoryCommand(inventory);
						break;
					case "jump":
						string newloc = JumpCommand(currentLocation, inventory);
						if (newloc != currentLocation)
						{
							currentLocation = newloc;
							DisplayRoom(currentLocation);
						}
						break;
					case "look":
						DisplayRoom(currentLocation);
						break;
					default:
						Console.WriteLine("I don't know what that is!");
						break;
				}
			}
		}

		/// <summary>
		/// Main Function. Start the game!
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			StartGame();
		}
	}
}
