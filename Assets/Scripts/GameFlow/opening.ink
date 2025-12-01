=== start
# narrator:
# image:kingdom_flourish 
Once upon a time, {kingdom_name} flourished. 

# image:kingdom_fade
But like all great kingdoms and empires, the period of peace and progress was soon brought to an end. An end marked by blood-stained stone walls and hushed whispers.

# image:three_kids
Among those left behind to rebuild in the aftermath were three children, tied by the past but destined for different paths. 

# image:diff_paths
One, by choice, took up the sword. Another, true to their nature, took up the pen. And the last, unwilling yet bound by duty, was forced to take up the crown.

# image:famine_strike
Years later, after a tentative peace had been established and a semblance of normalcy had been restored to {kingdom_name}, a famine threatens to sweep through the whole kingdom while the threat of an usurper looms on the horizon.

# image:famine_strike
There are three main threads in this tapestry, woven together by fate and choice. And now, you must decide which thread to follow. 

~ show_character_selection = true

+ [Sword]
    ~ player_choice = "Soldier"
    ~ show_character_selection = false
    -> soldier_prologue
+ [Pen]
    ~ player_choice = "Poet"
    ~ show_character_selection = false
    -> poet_prologue
+ [Crown]
    ~ player_choice = "King"
    ~ show_character_selection = false
    -> king_prologue
  

