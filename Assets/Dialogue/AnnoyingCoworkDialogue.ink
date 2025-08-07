
-> Gossip

VAR name0 = "Peter"
VAR name1 = "Peter"
VAR name2 = "Peter"
VAR name3 = "Peter"

== Game ==
Did you watch the game last night?
    * [Yes] Yeah, I saw it.
        That ending huh?
            ** What happened again?
                Just the play of the century!
            ** I know right?
                Really something.
    * [No] No, I didn't.
        Well why not?
            ** I don't really follow the sport.
            ** I was busy.
                What were you up to?
                    *** Nothing much.
                            C'mon what were you doing?
                                **** Just doing work I guess.
                                **** I went for a run
                                    Oh, that sucks.
                    *** I had to work late.
                            Huh, sounds like you had a rough night.
    - Well I'll catch you later. 
    -> Bye

== Gossip == 

Hey, did you hear about {name0}?
    * Yes.
        ->what_do_you_think
    * No.
        So basically {name1} told me that {name2} said that {name3} 
        overheard {name0} talking about the new product.
        ** What about it?
            {name1} told me that {name2} said that {name3} 
            overheard {name0} say it's gonna be green instead of orange.
                ***[That's cool]That's cool I guess.
                *** Huh.
        ** Oh yeah I heard that.
            ->what_do_you_think
- Well I'll see you around.
-> Bye

== what_do_you_think ==
    So, what do you think?
            *I think they're insane.
                Woah, I wouldn't go as far as to say that.
                    **Maybe I overreacted.
                        Yeah, you're kind of a jerk for that.
                    **I would.
                        Jeez, that's a little harsh.
                        Like what if {name0} said that to you?
                        How would that make you feel?
            *They were probably just having a bad day
                Yeah, just like you! always working all the time.
                I guess everyday is a bad day for you.
                Maybe that means everyday is good too because they're all the same.
                Eh, probably not.
    - Well I'll see you around.
-> Bye
                
== Advice ==
Whatcha doing bud?
    * I'm trying to work.
        Hey, no need for the attitude.
        You know, my doctor told me that working is bad for the soul.
        Sometimes you just gotta take it easy.
        ** Huh, I haven't thought of that.
            Yeah maybe you should try it sometime.
        ** This is my job.
            Hey, your body is a temple.
            Never forget that.
    * Just working on these docs.
        Cool, cool, cool.
        Hey, do you think you could get me a coffee.
        After you're done with those "docs" of course.
        ** Yeah I guess so.
            I knew you'd hook me up.
        ** Get it yourself.
            Always with the attitude.
            You really gotta work on that brochacho.
            I know this really good therapist.
            She specializes in anger management.
            
- Well I'll catch you on the flippity flop.
-> Bye

==Bye==
    I've got a lot of work to do!
-> END