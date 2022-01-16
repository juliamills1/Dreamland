// ---- FOOTSTEPS ---- //
global Event changeHappened;
// movement: 0 = stationary, 1 = walk, 2 = run
global int mode;
me.dir() + "walking.wav" => string walking;
me.dir() + "running.wav" => string running;
SndBuf buf => NRev n => Gain g => dac;
1 => buf.loop;
0.18 => n.mix;
1.5 => g.gain;

// ---- CLUSTERS ---- //
global Event clusterTrigger;
SndBuf cluster => NRev cn => Gain cg => dac;
0.8 => cluster.gain;
0.18 => cn.mix;
0.0 => cg.gain;
me.dir() + "clusters.wav" => cluster.read;
cluster.samples() => int numSamples;

// ---- MINICLUSTERS ---- //
global Event miniClusterTrigger;
global int miniOn; // outgoing
SndBuf minicluster => Gain mg => dac;
1.0 => minicluster.gain;
0.0 => mg.gain;
me.dir() + "minicluster.wav" => minicluster.read;
minicluster.samples() => int mNumSamples;

spork ~ clusters();
spork ~ miniClusters();

// footsteps according to player movement
while (true)
{
    // movement mode has changed
    changeHappened => now;
    0 => buf.pos;
    
    if (mode == 0)
    {
        // stationary: silent
        0 => buf.gain;
    }
    else if (mode == 1)
    {
        // use walk file in buffer
        walking => buf.read;
        0.8 => buf.gain;
    }
    else if (mode == 2)
    {
        // use run file in buffer
        running => buf.read;
        0.9 => buf.gain;
    }
}

// string clusters with progress down canyon
fun void clusters()
{
    // player has progressed past certain point
    clusterTrigger => now;
    0.3 => cg.gain;
    0 => cluster.pos;
    numSamples::samp => now;
        
    // at end of file
    if (cluster.pos() == numSamples)
    {
        0.7 => cg.gain;
        
        // loop last tenth of file
        while (true)
        {
            (0.9 * numSamples) $ int => int pos;
            pos => cluster.pos;
            (0.1 * numSamples)::samp => now;
            // increase gain each repetition, up to 0.9
            Math.min(cg.gain() + 0.1, 0.9) => cg.gain;
        }
    }
}

// small string clusters during camera rotation
fun void miniClusters()
{
    while (true)
    {
        // camera has begun rotating
        miniClusterTrigger => now;
        // tell Unity audio has been triggered
        1 => miniOn;
        0.8 => mg.gain;
        0 => minicluster.pos;
        mNumSamples::samp => now;
        
        // tell Unity file is done playing
        0 => miniOn;
    }
}
