// ============================================
// DISTRIBUTED DEFENSE - INCLUDES
// ============================================

=== distributed_defense
{soldier_name} divides the troops up into four smaller squads of twenty, fanning them out to cover most of the village, and then she leads one of those squads into battle at the square.

The moment she arrives, chaos greets her. Smoke fills the air and the heat is suffocating. 

Seven of the ten stationed guards still hold the attackers back, striking them down before they can cross over the barricades into the village, though their wounds speak to a fight that has not gone well. 

Of the missing three, there is no sign.

Desperate villagers swing makeshift weapons at the attackers, trying in vain to defend their homes. She turns her attention to the villagers.  

# speaker: {soldier_name}
# image: Resources/Portraits/soldier_sprite

“Stand down! Put out the fires!”

Then she jumps headfirst into the chaos, her troops following behind her. The square is full of noise. A cacophony of yells, the clash of steel, and the roar of the fire.

It is a tune she is familiar with and yet something feels wrong. she slashes one of the attackers across the arm, he doesn’t flinch.

she sees another take a spear through the stomach and only falter when the steel rips clear through his spine.

It hits her at once. Something is wrong with these men. Something dangerous. Something that’s keeping them from feeling or responding to pain.

# speaker: {soldier_name}
# image: Resources/Portraits/soldier_sprite

“Aim to kill!” 

Her voice thunders, echoed by her soldiers through the square. she'll have to forgo taking prisoners for questioning.

The battle rages for hours. {soldier_name}'s troops fight valiantly but the enemy feels endless. Without numbers on her side, the clash is brutal. For every bandit she cuts down, two more push through.

What’s worse is their refusal to fall from wounds that would cripple any normal man. {soldier_name}'s soldiers strain under the sheer numbers, and the cracks begin to show. Bandits slip past, attacking villagers and feeding the chaos.

Hours pass before the last bandit falls. {soldier_name} drops to one knee, exhausted, battered, and bruised. The village square is riddled with bodies, too many of them her own men and villagers.

She pushes herself back to her feet to take stock of the damage. Of the original ten stationed guards, eight are dead. From her twenty, twelve more have fallen and four lie critically wounded. Six villagers dead, ten more injured. A scout kneels before her, face grim

# speaker: Soldier
“Captain, no reinforcements could come. The next village… they were under attack as well. The bandits hit the food stores. They stole what they could, then burned the rest.”
 
Later, word comes back from the squads you sent out through the village. The fires were indeed set at the food storage sites and farms.

But the soldiers fought off the bandits there, managed to put out the flames, and kept the villagers’ homes from being destroyed.

Still, twelve of her scattered troops fell and twenty-two were badly injured.

Sixty eight out of a hundred soldiers remain alive with a large portion of that number injured.

It is not the worst number, when compared to the bodies of the bandits that litter the village, and the food and livestock secured. Yet the cost is heavy.

The soldiers are bloodied and dejected. Too many of their comrades were lost in one night. The villagers will be fed, but {soldier_name}'s troops are not the same.

Morale is low, and though the kingdom may have survived this day, war lurks on the horizon, and doubt in the Royal guard’s ability to win that war looms large.

What does she do next?

+ [Say a quiet prayer for the dead.]
    -> distributed_defense_prayer

+ [Get to work immediately.]
    -> distributed_defense_get_to_work 

=== distributed_defense_prayer
 She bows her head for a few seconds. Her eyes sting but not from the fire. For a moment, the battlefield goes unbearably still.

She whispers a short prayer, one her mother taught her when she were barely old enough to speak. One she had whispered over her parent's grave. 

It is not a soldier’s prayer. It is a human one.  It is also a promise, a promise that their lives and sacrifices will not be swallowed by silence.

When she finally opens her eyes, ash drifts like slow snow around her. She rises to her feet and keeps moving. Duty waits for no one.
-> distributed_defense_letter_aftermath
    
=== distributed_defense_get_to_work
She turns away from the bodies and the smoke before the weight can settle heavy on her shoulders. There is no time to linger.

She keeps her head high as she tries not to look at the death and destruction that surrounds her.

What's left of her soldiers glance at her, looking to her for certainty, for strength. She gives them what she can, even if it feels weak.

A captain does not have the luxury of mourning. Not when there is still so much left to be done.
-> distributed_defense_letter_aftermath



=== distributed_defense_letter_aftermath

The aftermath of the battle has {soldier_name} working late into the afternoon. By the time she returns to her tent, she finds two carrier pigeons perched on her sleeping bag.

The first one holds a paper with a response to her earlier message: there would be no reinforcements or food from bordering villages. They, too, had been attacked that day and were dealing with the aftermath of their own losses. 

The words strike a chord in her. If the border was hit in several places at once, then the question was not only who attacked them, but how they could muster an army so large and coordinated.

She forces the thought aside and looks to the second pigeon, which carries two rolls of parchment. One bears the crest of the royal army. The other bears a seal that makes her pause, one she has not seen in four years.

Which does sge open first?

+ [Open the letter from the royal army first.]
    -> distributed_defense_open_army_letter_first

+ [Open the letter with the familiar seal first.]
    -> distributed_defense_open_personal_letter_first

=== distributed_defense_open_army_letter_first
She breaks the royal army letter first. It details her orders. She is to leave the village four days earlier than planned, relieved of duty, and a new captain is already en route.

She is to return to the capital immediately and present herself before the King at the Palace. 

Her chest tightens at the thought of leaving her soldiers so soon after such heavy losses, but one did not disobey an order from a general, least of all one that seemed to have come from the King 

She hastily pens a reply on the back, giving a detailed report of the morning’s battle and requesting additional men or a faster rotation of the troops, so that her surviving soldiers can return earlier and receive proper medical attention. 

She ties the message to the bird’s leg, then turns to the second parchment. 

The royal crest. She takes it with a tentative hand. The letter inside is brief, a single line. She does not realize she has been holding her breath until it leaves her in a sigh, heavy and uncertain. 

She clenches the parchment tightly in her grasp even as she seals her response to the army and watches the pigeon fly off.

-> distributed_defense_after_letters

=== distributed_defense_open_personal_letter_first

She picks up the letter with the familiar seal, the royal crest, first, her hand trembling slightly. The letter inside is brief, a single line. 


She does not realize she has been holding her breath until it leaves her in a sigh, heavy and uncertain.

She clenches the parchment tightly in her grasp even as reaches for the letter from the army.

That letter details her orders. She is to leave the village four days earlier than planned, relieved of duty, and a new captain is already en route.

She is to return to the capital immediately and present herself before the King at the Palace. 

Her chest tightens at the thought of leaving her soldiers so soon after such heavy losses, but one did not disobey an order from a general, least of all one that seemed to have come from the King

She hastily pens a reply on the back, giving a detailed report of the morning’s battle and requesting additional men or a faster rotation of the troops, so that her surviving soldiers can return earlier and receive proper medical attention. 

She ties the message to the bird’s leg and watches the pigeon fly off.

-> distributed_defense_after_letters

=== distributed_defense_after_letters
The next three days are consumed with grief and labor. 

Able-bodied troops help tend to the injured and rebuild damaged homes and food stores. Most of the soldiers are wounded, their numbers gutted, and morale is low.

Though their food remains intact, villagers whisper their doubts and fears as they watch the cost paid by the royal guard.

Every day, she battles her own guilt — over dispersing the troops too thin, over the lives lost, over the erosion of faith in her leadership.

When the new captain arrives with reinforcements and word that aid will follow, she leaves. As she mounts her steed, the weight of her summons presses heavily on her shoulders.

Behind her, she leaves stores and fields intact, but the village is unsteady, its defenders broken, and its people uncertain. Ahead lies the capital, the Palace.

->  soldier_chapter2_distributed_defense