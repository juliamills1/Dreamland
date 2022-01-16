// ---- MAIN ARP SYNTH ---- //
global Event newTrigger;
SinOsc s => LPF l => NRev n => dac;
0 => s.gain;
20000 => l.freq;
0.02 => n.mix;

// arpeggiation patterns
[0, 3, 7, 10, 7, 3] @=> int p1[];
[10, 7, 3, 7, 3, 0] @=> int p2[];
[0, 7, 3, 10, 7, 3] @=> int p3[];
[10, 3, 7, 0, 3, 7] @=> int p4[];
[p1, p2, p3, p4] @=> int pitchClasses[][];
0 => int currentPitch;

// ---- STRING LAYERS ---- //
["E6.wav", "B7.wav", "E7.wav", "D8.wav"] @=> string layerSmp[];
SndBuf layerBufs[4];
NRev verb2;
0.2 => verb2.mix;
Gain g[4];
global Event changeHappened;
global int mode;
global Event layerChange;
global int runs;

// initialize layer sound buffers
for( int i; i <= 3; i++)
{
    layerBufs[i] => verb2 => g[i] => dac;
    me.dir() + layerSmp[i] => layerBufs[i].read;
    1 => layerBufs[i].loop;
    0.0 => g[i].gain;
}

// ---- TRIGGER SYNTH ---- //
global Event trigger;
TriOsc t => ADSR e => n => dac;
0.35 => t.gain;
e.set(10::ms, 8::ms, .5, 500::ms);

spork ~ arp();
spork ~ listenForLayerEdit();
spork ~ listenForTrigger();
spork ~ listenForNewTrigger();

// turn arp gain on/off according to player movement
while (true)
{
    // movement mode has changed
    changeHappened => now;
    
    if (mode == 0)
    {
        // stationary: fade out
        fadeVolume(0.2, 0.0);
    }
    else
    {
        // moving: fade in
        fadeVolume(0.0, 0.2);
    }
}

// increment volume towards target
fun void fadeVolume(float startingVol, float targetVol)
{
    for (1 => int i; i <= 20; i++)
    {
        startingVol + ((i / 20.0) * (targetVol - startingVol)) => s.gain;
        5::ms => now;
    }
}

// play arpeggiation patterns according to player movement
fun void arp()
{
    while (true)
    {
        // if currently moving
        if (mode != 0)
        {
            // get next pitch in current arpeggio pattern
            Std.mtof(52 + pitchClasses[mode-1][currentPitch]) => s.freq;
            currentPitch++;
            
            // reset cycle
            if (currentPitch >= pitchClasses[mode-1].size())
            {
                0 => currentPitch;
            }
        }
        
        0.12::second => now;
    }
}

// add string layers each maze run
fun void listenForLayerEdit()
{
    while (true)
    {
        // number of runs has changed
        layerChange => now;
        
        // if number of runs < number of string layers
        if (runs < 4)
        {
            // add next layer
            0 => layerBufs[runs].pos;
            0.2 => g[runs].gain;
        }
    }
}

// trigger triangle bloop on checkpoints
fun void listenForTrigger()
{
    while (true)
    {
        // checkpoint passed
        trigger => now;
        // set frequency to octave above current arp pitch
        s.freq() * 2 => t.freq;
        e.keyOn();
        200::ms => now;
        e.keyOff();
        e.releaseTime() => now;
    }
}

// decrease sine LFO frequency with maze progress
fun void listenForNewTrigger()
{
    while (true)
    {
        // new checkpoint passed
        newTrigger => now;
        l.freq() - 525 => l.freq;
    }
}

