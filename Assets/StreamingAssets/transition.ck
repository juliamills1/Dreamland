// ---- TICKING ---- //
SndBuf timer => NRev n => dac;
me.dir() + "timer.wav" => timer.read;
0.5 => timer.gain;
0.1 => n.mix;

// ---- TIMER RING ---- //
SndBuf ding => n => dac;
me.dir() + "ding.wav" => ding.read;
0.0 => ding.gain;
0 => global int triggered; // outgoing

// ---- FOOTSTEPS ---- //
SndBuf steps => NRev sn => dac;
me.dir() + "transitionSteps.wav" => steps.read;
0.0 => steps.gain;
1 => steps.loop;
0.2 => sn.mix;
global Event changeHappened;
global int walking;
global Event rateChange;
global float rate;

// play ticking file
0 => timer.pos;
timer.samples()::samp => now;
// tell Unity timer has "gone off"
1 => triggered;
// play timer ring file
0 => ding.pos;
0.2 => ding.gain;

spork ~ listenForRateChange();

// footsteps according to player movement
while (true)
{
    // movement mode has changed
    changeHappened => now;

    if (walking == 1)
    {
        // play footsteps file
        0 => steps.pos;
        0.4 => steps.gain;
    }
    else
    {
        // stationary: silent
        0 => steps.gain;
    }
}

// update footsteps playback rate according to progress
fun void listenForRateChange()
{
    while (true)
    {
        // rate has changed (from Unity)
        rateChange => now;
        rate => steps.rate;
    }
}

    