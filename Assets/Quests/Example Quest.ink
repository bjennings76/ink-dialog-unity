VAR found_berries = false

-> Forest

=== Forest ===
#left:Player
#right:Ron
#back:Castle

<h3> An Example </h3>
<h4> Part 1 </h4>
(You follow a flickering light.)
(Someone has lit a torch out in the woods.)

+ <i>Hello? Is anyone there?</i>
+ <i>Who's out there?!</i>

- Huh? Oh, hi Harry! It's me, Ron!
What are you doing way out here, Harry?

+ <i>I was just about to ask you the same thing!</i>
That's funny. Great minds think alike, eh?

+ <i>Me?! What about you?!</i>
Oh, you know...

+ <i>I saw your torch[...] and wondered who was out here. You?</i>
Oh yeah... I guess it can be pretty bright, huh?

- Well my owl is sick and I wanted to get him his favorite treat.
There are berries out here that he loves. Little red ones. 
Do you see any about? I can't find any.

+ <i>[Time for dinner] No, Ron. I haven't seen any berries. Let's get back to dinner asap.</i>
+ <i>[I can help] No, but I could have a look around. Wait here for me?</i>
    You would do that?! Thank you so much!
    -> Gathering

- Okay... I guess we can head back. . -> Dinner

=== Gathering ===
<h4> Part 2 </h4>

- (start)

#left:Player
#right:
#back:Castle
+ { not found_berries } [({Gather Berries|Gather more berries.|Gather even more berries.})] (You gather {|more |even more }berries from a bush nearby.{ How Ron could have missed it is beyond you.|})
    ~ found_berries = true
    
+ { found_berries } [(Eat Berries)] (You eat the berries.)
    ~ found_berries = false
+ \(Find Ron) -> approach

- -> start

= approach
#left:Player
#right:Ron
#back:Castle
{
    - found_berries:
        + <i>Okay, I found some.[] Hopefully Pigwidgeon likes them!</i>
        -> give_food
    - else:
        Did you find any berries?
        + None yet, sorry. 
        Okay. Let me know when you do.
        
        -> start
}

= give_food

~ found_berries = false

- Wow! You are amazing!
My Pigwidgeon will be so happy!

+ <i>No problem at all, Ron.[] Now let's head back.
+ <i>Yeah, well... I'm starving.[] Let's get back to dinner.

-> Dinner

=== Dinner ===
#left:
#right:Ron
#back:DiningHall
{ found_berries:
    Just in time for dinner! Pigwidgeon and I are going to be stuffed!
- else:
    Glad to be back... but Pigwidgeon is going to be angry with me.
}

#left:Player
+ <i>...</i>

(Just for fun, let's try that again...)
~ found_berries = false
-> Forest
