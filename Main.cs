using System;
using Microsoft.CSharp;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Thanks to 
/// http://codereview.stackexchange.com/users/38054/benvlodgi
/// http://codereview.stackexchange.com/users/4318/eric-lippert
/// http://codereview.stackexchange.com/users/23788/mats-mug
/// http://codereview.stackexchange.com/users/30346/chriswue
/// </summary>
namespace RPSLSGame
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			/* Here are your rules: 
			"Scissors cuts paper, 
			paper covers rock, 
			rock crushes lizard, 
			lizard poisons Spock, 
			Spock smashes scissors, 
			scissors decapitate lizard, 
			lizard eats paper, 
			paper disproves Spock, 
			Spock vaporizes rock. 
			And as it always has, rock crushes scissors." 
			-- Dr. Sheldon Cooper */
			int win = 0;
			int lose = 0;
			int tie = 0;

			IList<Gesture> gameGestures = GetGestures();

			do{
				Console.WriteLine("Please choose your Gesture ");
				PrintMenu(gameGestures);
				var playerGesture = gameGestures[PromptForNumber("Please choose your gesture",1,gameGestures.Count)-1];
				var computerPlay = GetRandomOption(gameGestures);

				Console.WriteLine ("Computer: " + computerPlay);
				Console.WriteLine ("your Gesture: " + playerGesture);

				if (playerGesture.Equals(computerPlay)){
					tie++;
				}else if(playerGesture.Defeats(computerPlay)){
					win++;
				}else{
					lose++;
				}

				Console.WriteLine ("Your Score is (W:L:T:) : {0}:{1}:{2}", win, lose, tie);
				if (Choice("Would you like to reset your score?"))
				{
					win = 0;
					lose=0;
					tie=0;
				}
			}while (Choice("Would you like to play again? Y/N"));
			Console.WriteLine("Goodbye");
			Console.ReadLine ();
		} 

		public static void DecideWinner ()
		{
			//TODO: Create Method for Deciding the Winner.
			//I still need to move the decision logic here. 
			//but it won't be as big as I thought it was going to be
		}

		static int PromptForNumber (string prompt, int lower, int upper)
		{
			int? pick = null;
			while (pick == null) {
				Console.WriteLine(prompt);
				pick = Console.ReadLine().BoundedParse (lower, upper);
			}
			return pick.Value;
		}

		public static void PrintMenu (List<string> List)
		{
			for (int i=0; i<List.Count; i++) {
				Console.WriteLine ((i+1) + ": " + List[i]);
			}		
		}

		public static void PrintMenu (IList<Gesture> GestureList)
		{
			for (int i=0; i < GestureList.Count; i++) {
				Console.WriteLine ((i + 1) + ": " + GestureList [i]);
			}
		}

		public static string GetRandomOption (List<string> options)
		{
			Random rand = new Random();
			return options[rand.Next(0,options.Count)];
		}

		public static Gesture GetRandomOption (IList<Gesture> options)
		{
			Random rand = new Random();
			return options[rand.Next (0,options.Count)];
		}

		public static Boolean Choice (string prompt)
		{
			while(true)
			{
				Console.WriteLine (prompt);
				switch (Console.ReadKey (true).Key)
				{
					case ConsoleKey.Y:
					{
						Console.Write ("Y\n"); 
						return true;
					}
					case ConsoleKey.N:
					{
						Console.Write ("N\n");
						return false;
					}
				}
			}
		}
	
/// <summary>
/// This is some code that I borrowed from 
/// http://codereview.stackexchange.com/users/14749/peter-kiss
/// from an answer that he gave to one of my Questions, the answer has since been deleted.
/// </summary>

		static IList<Gesture> GetGestures()
		{
        	var spock = new Gesture("Spock");
        	var lizard = new Gesture("Lizard");
        	var paper = new Gesture("Paper");
        	var rock = new Gesture("Rock");
        	var scissors = new Gesture("Scissors");

	        spock.AddDefeats(new[] { scissors, rock });
    	    lizard.AddDefeats(new[] { paper, spock });
        	paper.AddDefeats(new[] { rock, spock });
        	rock.AddDefeats(new[] { lizard, scissors });
        	scissors.AddDefeats(new[] { paper, lizard });

	        var gestures = new List<Gesture> {rock, paper, scissors, lizard, spock};
    	    gestures.ForEach(x => x.Seal());
        	return gestures;
    	}
	}

	public class Gesture
	{
    	public override string ToString()
	    {
    	    return Name;
	    }

	    private HashSet<Gesture> _defeats = new HashSet<Gesture>();
    	private bool _isSealed;
    	public string Name { get; private set; }

    	public Gesture(string name)
    	{
        	if (name == null) throw new ArgumentNullException("name");
        	Name = name;
    	}

    	public void Seal()
    	{
        	_isSealed = true;
    	}

    	public void AddDefeats(IEnumerable<Gesture> defeats)
    	{
        	if (_isSealed)
        	{
            	throw new Exception("Gesture is sealed");
        	}

        	foreach (var gesture in defeats)
       		{
        	    _defeats.Add(gesture);
    	    }
    	}

		//public void AddKillMessage(Tuple<Gesture,Gesture,string>  
		public void AddKillMessage (Dictionary<Dictionary<Gesture,Gesture>,string> killMessage)
		{
			if (_isSealed) {
				throw new Exception ("Gesture is Sealed");
			}
		}

	    protected bool Equals(Gesture other)
	    {
    	    return string.Equals(Name, other.Name);
	    }

    	public override bool Equals(object obj)
    	{
    	    if (ReferenceEquals(null, obj)) return false;
    	    if (ReferenceEquals(this, obj)) return true;
    	    if (obj.GetType() != this.GetType()) return false;
    	    return Equals((Gesture) obj);
    	}

    	public override int GetHashCode()
        {
    	    return Name.GetHashCode();
	    }

    	public bool Defeats(Gesture gesture)
    	{
        	return _defeats.Any(x => x.Equals(gesture));
    	}
	}
}

static class Extensions
{
	public static int? BoundedParse (this string str, int lower, int upper)
	{
		int result;
		bool success;
		success = int.TryParse (str, out result);
		if (!(lower <= result && result <= upper) || (!success) || (str == null)){
			return null;
		}
		return result;
	}
}
