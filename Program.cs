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
					{"items", new List<string>{"flare"} }
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

			{"flare", new Dictionary<string, object>
				{
					{"title", "A wee emergency flare gun" },
					{"desc", "It is ready to fire. Best do this at the right place" },
				}
			}

		};


		static List<string> COMMANDS = new List<string>
		{
			"north", "south", "east", "west", "drop", "eat", "get", "inventory", "inv", "jump", "look",
		};


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
			if (ROOMS[location].ContainsKey("items"))
			{
				Console.WriteLine("You See:");
				foreach (string item in (List<string>)ROOMS[location]["items"])
				{
					Console.WriteLine($"\t{ITEMS[item]["title"]}");
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
