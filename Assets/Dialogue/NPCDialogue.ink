VAR nextPerson = "Peter Marsh"
VAR needToReview = false
VAR keepInLoop = false
->Waiting
==Idle==
-I think you're looking for someone else.
-> END

==Waiting==
{needToReview: Come back later, I think these still need reviewing |{ shuffle:
	 -Thanks, I've got a few edits.
	 -Thank you. I'll be done soon.
}}

-> END

==Editing==
{ shuffle:
	 -Docs are ready.
	 -I'm done.
	 -I've got those docs here.
}

{ shuffle:
	 -After they're reviwed, bring these to {nextPerson}.
	 -After review, {nextPerson} needs these.
	 -Take these docs over to {nextPerson} after they're reviewed.
}

* Ok.
* Alright.

-{keepInLoop: Also make sure to keep me in the loop on these docs}


--> END


==Reviewing==
Ready for review?
* Yes.
* Yup.

-Hmmm

These Look good to me.

{needToReview: Make sure everyone else in the loop gets their eyes on these.| Bring these to {nextPerson}.}
* Ok.
* Alright.
--> END