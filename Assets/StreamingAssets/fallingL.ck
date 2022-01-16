SndBuf buf1 => NRev n => dac;
0.08 => n.mix;

// load & play first file
me.dir() + "submixL1.wav" => buf1.read;
0 => buf1.pos;
3 => buf1.gain;
15::second => now;

// load & play second file
SndBuf buf2 => n => dac;
me.dir() + "submixL2.wav" => buf2.read;
0 => buf2.pos;
2 => buf2.gain;
// play to the end of the file
buf2.samples()::samp => now;