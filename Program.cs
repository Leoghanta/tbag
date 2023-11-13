using System.ComponentModel.Design;
using System.Reflection;

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

		static Dictionary<string, Dictionary<string, string>> ROOMS = new Dictionary<string, Dictionary<string, string>>
		{
			//Start
			{"start", new Dictionary<string, string>
				{
					{"title", "The Start of the Maze" },
					{"desc", "You are at the start of a large maze" },
					{"east", "maze" },
				}
			},

			//ROOM1
			{"maze", new Dictionary<string, string>
				{
					{"title", "Lost in the Maze" },
					{"desc", "Ah crap! You're lost in the maze!" },
					{"west", "start" },
				}
			}
		};

		static List<string> COMMANDS = new List<string>
		{
			"north", "south", "east", "west", "look",
		};


		static void DisplayRoom(string location)
		{
			Console.WriteLine(ROOMS[location]["title"]);
			Console.WriteLine(ROOMS[location]["desc"]);
            Console.WriteLine("Obvious Exists are:");
            if (ROOMS[location].ContainsKey("north") )
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

		static string[] GetPlayerInput()
		{
			while (true)
			{
                Console.Write(">> ");
                string input = Console.ReadLine().ToLower();
				string[] inputsplit = input.Split(" ");

				if (COMMANDS.Contains(inputsplit[0])){
					return inputsplit;
				}
				else
				{
                    Console.WriteLine("I don't know what that means!");
                }
			}
		}

		static string TravelTo(string currentlocation, string direction)
		{
			if (ROOMS[currentlocation].ContainsKey(direction))
			{
				Console.WriteLine($"You travel {direction}");
				return ROOMS[currentlocation][direction];
			}
			else
			{
				Console.WriteLine("You can't move in that direction");
				return currentlocation;
			}
		}

		 
		static void StartGame()
		{
			string currentLocation = "start";
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
					case "look":
						DisplayRoom(currentLocation);
						break;
					default: 
						Console.WriteLine("I don't know what that is!");
						break;
				}
			}
		}


		static void Main(string[] args)
		{
			StartGame();
		}
	}
}
