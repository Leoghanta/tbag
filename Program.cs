namespace tbag
{
	internal class Program
	{
		static Dictionary<string, Dictionary<string, object>> ROOMS = new Dictionary<string, Dictionary<string, object>>
	{
		{ "start", new Dictionary<string, object>
			{
				{"title", "The Start!" },
				{"desc", "You are at the start of a maze" },
				{"east", "room1" }
			}
		},
		{ "room1", new Dictionary<string, object>
			{
				{"title", "You are in room 1" },
				{"desc", "The first room is here - try jumping" },
				{"items", new List<string> { "umbrella" } },
				{"west", "start" },
				{"south", "room2" }
			}
		},
		{ "room2", new Dictionary<string, object>
			{
				{"title", "You are in room 1" },
				{"desc", "You are in another room" },
				{"north", "room1" },
				{"east", "end" }
			}
		},
		{ "end", new Dictionary<string, object>
			{
				{"title", "You have escaped" },
				{"desc", "The maze has ended!" },
				{"west", "room2" }
			}
		},
		{ "secret", new Dictionary<string, object>
			{
				{"title", "You are in the Secret Room" },
				{"desc", "You found a secret room. Continue south to finish your quest" },
				{"south", "end" }
			}
		}
	};

		static Dictionary<string, Dictionary<string, string>> ITEMS = new Dictionary<string, Dictionary<string, string>>
	{
		{ "umbrella", new Dictionary<string, string>
			{
				{"name", "An umbrella" },
				{"desc", "A cute umbrella with the words M.POPPINS engraved on it." }
			}
		}
	};

		static string[] COMMANDS = { "north", "south", "east", "west", "jump", "look", "help", "get", "inventory" };
		static string[] MOVEMENT = { "north", "south", "east", "west" };

		// display_ROOMS
		// function to display the current ROOMS
		static void DisplayRooms(string loc)
		{
			Console.WriteLine(ROOMS[loc]["title"]);
			Console.WriteLine(ROOMS[loc]["desc"]);
			if (ROOMS[loc].ContainsKey("items"))
			{
				Console.WriteLine("You See...");
				foreach (var thing in (List<string>)ROOMS[loc]["items"])
				{
					Console.WriteLine($"\t{ITEMS[thing]["name"]}");
				}
			}
			Console.WriteLine("Obvious exits are:");
			if (ROOMS[loc].ContainsKey("north"))
				Console.WriteLine($"\tnorth: {ROOMS[loc]["north"]}");
			if (ROOMS[loc].ContainsKey("east"))
				Console.WriteLine($"\teast: {ROOMS[loc]["east"]}");
			if (ROOMS[loc].ContainsKey("south"))
				Console.WriteLine($"\tsouth: {ROOMS[loc]["south"]}");
			if (ROOMS[loc].ContainsKey("west"))
				Console.WriteLine($"\twest: {ROOMS[loc]["west"]}");
		}

		// JUMP COMMAND
		// jump in the location
		// check the inventory if required.
		static string JumpCommand(string loc, List<string> inv)
		{
			if (loc == "room1")
			{
				if (inv.Contains("umbrella"))
				{
					Console.WriteLine("You jump up high\nThe umbrella suddenly opens and you float down safely");
					Console.WriteLine("You land carefully and notice you are somewhere completely different!");
					return "secret";
				}
				else
				{
					Console.WriteLine("You jump up high\nYou begin to fall, fall, fall...");
					Console.WriteLine(">SPLAT< - not a textbook landing");
					KillPlayer("Landed on your head");
				}
			}
			Console.WriteLine("You jump about menacingly");
			return loc;
		}

		// Kill Player
		// Death is inevitable
		static void KillPlayer(string reason)
		{
			Console.WriteLine(new string('-', 80));
			Console.WriteLine($"\tYou have died - {reason}");
			Console.WriteLine(new string('-', 80));
			Environment.Exit(0);
		}

		// GET INPUT
		// function to get the input and split the command line
		static string[] GetInput()
		{
			while (true)
			{
                Console.Write(">> ");
                string[] usercommand = Console.ReadLine().ToLower().Split(" ", 2);
				if (Array.IndexOf(COMMANDS, usercommand[0]) > -1)
				{
					return usercommand;
				}
				else
				{
					Console.WriteLine("I don't know that command");
				}
			}
		}

		// GET ITEM
		// pick up an item and put it in inventory
		static string GetCommand(string loc, string thing)
		{
			if (ITEMS.ContainsKey(thing))
			{
				if (((List<string>)ROOMS[loc]["items"]).Contains(thing))
				{
					Console.WriteLine($"You pick up the {thing}");
					((List<string>)ROOMS[loc]["items"]).Remove(thing);
					return thing;
				}
				else
				{
					Console.WriteLine($"The {thing} isn't here!");
					return "";
				}
			}
			else
			{
				Console.WriteLine($"The {thing} doesn't exist!");
				return "";
			}
		}

		// TRAVEL from loc to direction
		// function to check if travel is possible and return new location
		// return "" if not successful.
		static string Travel(string loc, string direction)
		{
			if (ROOMS[loc].ContainsKey(direction))
			{
				Console.WriteLine($"You move {direction}");
				return (string)ROOMS[loc][direction];
			}
			else
			{
				Console.WriteLine($"You cannot go {direction}");
				return "";
			}
		}

		// MAIN FUNCTION
		// set up and game loop
		static void Main(string[] args)
		{
			int strength = 200;
			string loc = "start";
			List<string> inventory = new List<string>();

			DisplayRooms(loc);
			while (strength > 0)
			{
				string[] command = GetInput();

				if (Array.IndexOf(MOVEMENT, command[0]) > -1)
				{
					string newloc = Travel(loc, command[0]);
					if (newloc != "")
					{
						loc = newloc;
						DisplayRooms(loc);
					}
				}
				if (command[0] == "get")
				{
					string thing = GetCommand(loc, command[1]);
					if (thing != "")
					{
						inventory.Add(thing);
					}
				}
				if (command[0] == "inventory")
				{
					Console.WriteLine(string.Join(", ", inventory));
				}
				if (command[0] == "help")
				{
					Console.WriteLine(string.Join(", ", COMMANDS));
				}
				if (command[0] == "jump")
				{
					string newloc = JumpCommand(loc, inventory);
					if (newloc != loc)
					{
						loc = newloc;
						DisplayRooms(loc);
					}
				}
				if (command[0] == "look")
				{
					DisplayRooms(loc);
				}
			}
		}
	}
}