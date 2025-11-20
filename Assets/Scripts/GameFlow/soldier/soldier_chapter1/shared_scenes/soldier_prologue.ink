=== soldier_prologue
On the eve of the day meant to mark one final week before the troops’ return to the capital, the village under the protection of Captain {soldier_name}'s unit comes under attack.

The assault is sudden. The air of the border village is pierced by the blare of a blowhorn, and a red streak of flare cuts starkly across the evening sky. 

From the makeshift watchtowers, soldiers spot the faint tendrils of smoke, a telltale sign of fires scattered throughout the village. 

But the flare and blowhorn come from the heart of the village, where a thicker, darker column of smoke begins to stain the sky.

A soldier stationed at the village square rushes toward you, their uniform streaked with soot and blood.

# speaker: Soldier
Captain! The village square is under attack! Bandits — we believe they’re responsible for the fires across the village. There are too many to count; they keep flooding in. They’re not particularly skilled, but even so, our stationed guards can barely hold them back!

With only eighty soldiers at hand, the other twenty already on patrol, Captain {soldier_name}, must act quickly and decisively. She first sends off her personal carrier pigeon with a note to the captain of the troops at the closest village.


Then for good measure, she sends a human messenger on one of her fastest steeds in hopes of obtaining backup and issuing a warning.


Now you must choose her next course of action. 

+ [Concentrated Strike]
Focus most of her troops on the village square to confront the bandits, leaving only a few soldiers to cover the rest of the village. 
    ~ soldier_choice_1 = "concentrated"
    -> concentrated_strike
    
+ [Distributed Defense] 
Spread her troops across the village to defend multiple areas and investigate the other fires, taking only a few soldiers to the village square.
  
    ~ soldier_choice_1 = "distributed"
    -> distributed_defense

