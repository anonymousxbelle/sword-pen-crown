=== soldier_chapter2_concentrated_strike

The ride to the capital from the border would usually take five days—seven to eight if traveling with a troop, as the Captain had when she made the trip the first time. But the letter burning in the pocket sewn into the lining of her tunic drove her harder. 

She covered the distance in three and a half days, and would have ridden faster if not for the fear of killing her horse, the trusty steed she’d had since she was fourteen. 

The words from the letter kept turning over in her head, refusing to settle: ***the branch is breaking***. They pushed her on

It is late afternoon when {soldier_name} reaches the capital gates and is swept immediately toward the palace. 

Her horse is taken by a subordinate, her eyes barely register—hopefully to the royal stables—and she is ushered into a carriage that winds through the capital to the palace gates.

She alights, presents the letter sealed with the King’s crest, and flashes her Captain’s token: a small signet ring dangling from a plain metal chain around her neck.

The guards let her through without hesitation. Her mind catches on how simple the process is. Instinct makes her bristle at such lax security—especially after what happened seven years ago—but she forces the thought away.

The King had summoned her. That was explanation enough.

At the palace steps, an older lady-in-waiting greets her. Her face stirs a flicker of familiarity, though the Captain cannot place it.

She leads {soldier_name} through corridors she remembers sneaking through as a child, past portraits that seem to hold blood and grief in their painted eyes.

Nostalgia nearly chokes her when they arrive at a bedchamber that is not her old room, but looks enough like it to sting.

She is given time to wash. The grime and dried flakes of blood peel away under her scrubbing hands.

Ten minutes later, soaking in the great porcelain tub, she feels less like a captain returning to a haunted palace and more like a small girl again.

Her gaze drifts to the far side of the tub, where she half-expects another girl to be splashing in the water with her. 

The empty porcelain stabs her heart, and that pain is what forces her out. She dries herself quickly, refusing to linger in the nostalgia.

Dressed now in the fine, clean clothes laid out for her, she follows the lady-in-waiting down a quiet corridor. 

Each step feels heavier than the last and the door at the end of the corridor looks more daunting than she remembers.

Before she reaches it, {soldier_name} hesitates for half a breath.

+ [{soldier_name} steadies herself. She is a captain first.]
    ~ feeling_state = "duty"
    Duty settles over her shoulders like armor. She forces her breath steady, ready to face whatever waits inside.
    -> chamber_entry_bridgedis

+ [{soldier_name} lets the past catch up to her for a moment.]
    ~ feeling_state = "memory"
    A flicker of old memories presses at her ribs: laughter, voices, a warmth she hasn’t felt in years. She allows the ache, just for a heartbeat.
    -> chamber_entry_bridgedis


=== chamber_entry_bridgedis
The lady-in-waiting opens the door and she is guided into a small chamber off the King’s private suite. 

The wood-paneled room, with its table, single window, and armchairs arranged for privacy, is intimate in a way the throne room could never be. The air tastes of old heartache.

{king_name} and {poet_name}  are already there. They stand as {soldier_name} enters, as if pulled by instinct back into the shape of a trio. They are different from her memories of them. 

{ 
  - feeling_state == "duty":
        Her posture is straight, every movement controlled, the perfect soldier walking into the room.
  - feeling_state == "memory":
        The sight of them knocks something loose in her chest—familiar, painful, almost tender.
}


Her eyes move first to {poet_name} . Shoulders square, posture impeccable, straighter than her mother would have ever imagined.

Her once-long hair is shorn close, and the resemblance to her cousin is so striking that for a moment {soldier_name} almost believes the lie of his existence. But when {king_name} looks at her, the truth glimmers. 

She sees the girl who once climbed orchard walls and apple trees by her side. Hardened, yes but the softness still remained.

Their eyes lock, and in that wordless exchange lies everything the last four years had denied them: I’ve missed you. I’m sorry.

Her name slips from {soldier_name}’s lips before she can stop it, soft and heavy with habit. 

# speaker: {soldier_name}
# image: Resources/Portraits/soldier_sprite
{king_name}...

The King stiffens almost imperceptibly. The lady-in-waiting, still lingering, bows hastily and scurries out. {poet_name} lets out a laugh, sharp and edged with sarcasm but not unkind. 

# speaker: {poet_name}
# image: Resources/Portraits/poet_sprite
“Seven years and still not used to the name change, are you?”

His flint-sharp eyes and sly smile twist something warm in her chest—a warmth she hadn’t realized she’d been missing. But {king_name}s jaw ti

ghtens at his words, her silence heavy. For a breath, though, it feels as if nothing has changed. They are the same three children, only tempered by years they can never reclaim.

# speaker: {king_name}
# image: Resources/Portraits/king_sprite
"I'm sure you're wondering why I summoned you here," 

{king_name} said, effectively cutting off whatever else {poet_name} was going to say. But of course, {poet_name} would never let it go without getting the last word. 

# speaker: {poet_name}
# image: Resources/Portraits/poet_sprite
"If {soldier_name} couldn't piece together that we're here because of the war you and your council have been trying so hard to keep under wraps, then we should really question the competencies of the captains in our royal army."

Both {soldier_name} and {king_name} elect to ignore {poet_name}, a habit they'd honed years before, {soldier_name} more so than {king_name}. {soldier_name} noticed a flicker of something flash across {king_name}'s eyes. 

There was more truth to the {poet_name}'s jibe than snark.

# speaker: {king_name}
# image: Resources/Portraits/king_sprite
 "I heard news of the village you were stationed at, {soldier_name}. Three other villages reported attacks, but yours was hit the hardest by far. Your subordinates reported that you cut down more of the attackers than anyone else. You should be proud." 
 
 She did not smile, but something close to one tugged at the corner of her mouth, pride, sorrow, and some other weight {soldier_name} could not decipher. 

{soldier_name} felt the words sink like a stone in her chest. 

# speaker: {soldier_name}
# image: Resources/Portraits/soldier_sprite
"There is little pride to be felt when my choices left the villagers starving and some of them dead." 

She said bitterly.

-> chapter3_intro