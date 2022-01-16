TubeBell tb => JCRev jc => Gain g => dac;
0.2 => tb.lfoDepth;
0.8 => jc.mix;
0.22 => g.gain;

// pitch classes to choose from
[0, 1, 4, 7, 8, 11] @=> int scale[];
8 => int loopDimens;
dur durs[loopDimens];
float pitches[loopDimens];

spork ~ drone();

while (true)
{
    // set loop pitches
    for (int i; i < loopDimens; i++)
    {
        // randomize duration
        Math.random2f(0.2, 2.0) * 1.2::second => durs[i];
        // choose from pitch classes
        scale[Math.random2(0, 5)] => int freq;
        // randomize octave & convert to frequency
        Std.mtof( 51 + (Math.random2(0, 1)*12 + freq) ) => pitches[i];
    }
    
    // play loop x4
    for (int j; j < 5; j++)
    {
        for (int i; i < loopDimens; i++)
        {
            pitches[i] => tb.freq;
            // randomize volume
            tb.noteOn(Math.random2f(0.1, 0.4));
            durs[i] => now;
        }
    }
}

fun void drone()
{
    3 => int nInsts;
    SinOsc insts[nInsts];
    JCRev jcd => dac;
    0.1 => jcd.mix;
    
    // three octaves, decreasing in gain
    for (int i; i < nInsts; i++)
    {
        Std.mtof(27 + (12 * (i))) => insts[i].freq;
        0.11 - (0.03 * i) => insts[i].gain;
        insts[i] => jcd;
    }
    
    // play infinitely
    while (true)
    {
        1::minute => now;
    }
}



