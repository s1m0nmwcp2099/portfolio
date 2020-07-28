from mingus.midi import fluidsynth

fluidsynth.init("soundfont.SF2")

fluidsynth.play_Note(64,1,100)