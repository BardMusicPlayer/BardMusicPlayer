﻿#region

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro;

public sealed class MidiExport
{
    public static Encoding ascii = Encoding.ASCII;

    public int fileType = 1;

    public List<MidiTrack> midiTracks = new();
    public int ticksPerBeat = 960;

    public MidiExport(int fileType = 1, int ticksPerBeat = 960)
    {
        this.fileType = fileType;
        this.ticksPerBeat = ticksPerBeat;
    }

    public List<byte> createBytes()
    {
        var data = new List<byte>();
        data.AddRange(createHeader());
        foreach (var track in midiTracks) data.AddRange(track.createBytes());

        return data;
    }

    public IEnumerable<byte> createHeader()
    {
        var data = new List<byte>();

        var header = new List<byte>();
        header.AddRange(toBEShort(fileType));
        header.AddRange(toBEShort(midiTracks.Count));
        header.AddRange(toBEShort(ticksPerBeat));

        data.AddRange(writeChunk("MThd", header));

        return data;
    }

    public static IEnumerable<byte> writeChunk(string name, List<byte> data)
    {
        var _data = new List<byte>();

        _data.AddRange(ascii.GetBytes(name));
        _data.AddRange(toBEULong(data.Count));
        _data.AddRange(data);
        return _data;
    }

    public static IEnumerable<byte> toBEULong(int val)
    {
        var data = new List<byte>();
        var LEdata = BitConverter.GetBytes((uint)val);

        for (var x = LEdata.Length - 1; x >= 0; x--) data.Add(LEdata[x]);

        return data;
    }

    public static IEnumerable<byte> toBEShort(int val)
    {
        var data = new List<byte>();
        var LEdata = BitConverter.GetBytes((short)val);

        for (var x = LEdata.Length - 1; x >= 0; x--) data.Add(LEdata[x]);

        return data;
    }

    public static IEnumerable<byte> encodeVariableInt(int val)
    {
        if (val < 0) throw new FormatException("Variable int must be positive.");

        var data = new List<byte>();
        while (val > 0)
        {
            data.Add((byte)(val & 0x7f));
            val >>= 7;
        }

        if (data.Count > 0)
        {
            data.Reverse();
            for (var x = 0; x < data.Count - 1; x++) data[x] |= 0x80;
        }
        else
        {
            data.Add(0x00);
        }

        return data;
    }
}

public sealed class MidiTrack
{
    public List<MidiMessage> messages = new();

    public IEnumerable<byte> createBytes()
    {
        var data = new List<byte>();
        byte runningStatusByte = 0x00;
        var statusByteSet = false;
        foreach (var message in messages)
        {
            if (message.time < 0) message.time = 0;

            data.AddRange(MidiExport.encodeVariableInt(message.time));
            if (message.type.Equals("sysex"))
            {
                statusByteSet = false;
                data.Add(0xf0);
                data.AddRange(MidiExport.encodeVariableInt(message.data.Length + 1));
                data.AddRange(message.data);
                data.Add(0xf7);
            }
            else
            {
                var raw = new List<byte>();
                raw = message.createBytes();

                var temp = raw[0];
                if (statusByteSet && !message.is_meta && raw[0] < 0xf0 && raw[0] == runningStatusByte)
                {
                    raw.RemoveAt(0);
                    data.AddRange(raw);
                }
                else
                {
                    data.AddRange(raw);
                }

                runningStatusByte = temp;
                statusByteSet = true;
            }
        }

        return MidiExport.writeChunk("MTrk", data);
    }
}

public sealed class MidiMessage
{
    private readonly byte code;

    private readonly bool is_major = true;

    //instrument_name 0x04 -> track_name
    //lyrics 0x05 -> text
    //marker 0x06 -> text
    //cue_marker 0x07 -> text
    //device_name 0x08 -> track_name
    //channel_prefix 0x20
    public int channel;
    public int clocks_per_click = 24;

    //control_change 0xb0 (channel, control, value)
    public int control;

    //sysex 0xf0 (data)
    public byte[] data;
    public int denominator = 2;

    public bool is_meta;

    //key_signature 0x59
    public int key;

    //copyright 0x02 -> text
    //track_name 0x03
    public string name = "";
    public int notated_32nd_notes_per_beat = 8;

    //Messages:
    //#########
    //note_off 0x80 (channel, note, velocity)
    public int note;

    //MetaMessages:
    //#############
    //sequence_number 0x00
    public int number;

    //smpte_offset 0x54 (Ignore)
    //public int frame_rate = 24; public int hours = 0; public int minutes = 0; public int seconds = 0; public int frames = 0; public int sub_frames = 0;
    //time_signature 0x58
    public int numerator = 4;

    //aftertouch 0xd0 (channel, value)
    //pitchwheel 0xe0 (channel, pitch)
    public int pitch;

    //midi_port 0x21
    public int port;

    //program_change 0xc0 (channel, program)
    public int program;

    //end_of_track 0x2f
    //set_tempo 0x51
    public int tempo = 500000;

    //text 0x01
    public string text = "";
    public int time;

    public string type = "";

    //note_on 0x90 (channel, note, velocity)
    //polytouch 0xa0 (channel, note, value)
    public int value;
    public int velocity;

    //Others not needed..
    public MidiMessage(string type, IReadOnlyList<string> args, int time, byte[] data = null)
    {
        is_meta = false;
        this.type = type;
        this.time = time;

        switch (type)
        {
            //Meta Messages
            case "sequence_number":
                is_meta = true;
                code = 0x00;
                number = int.Parse(args[0]);
                break;
            case "text":
            case "copyright":
            case "lyrics":
            case "marker":
            case "cue_marker":
                is_meta = true;
                text = args[0];
                break;
        }

        switch (type)
        {
            case "text":
                code = 0x01;
                break;
            case "copyright":
                code = 0x02;
                break;
            case "lyrics":
                code = 0x05;
                break;
            case "marker":
                code = 0x06;
                break;
            case "cue_marker":
                code = 0x07;
                break;
            case "track_name":
            case "instrument_name":
            case "device_name":
                is_meta = true;
                code = 0x03;
                name = args[0];
                break;
        }

        switch (type)
        {
            case "instrument_name":
                code = 0x04;
                break;
            case "device_name":
                code = 0x08;
                break;
            case "channel_prefix":
                code = 0x20;
                channel = int.Parse(args[0]);
                is_meta = true;
                break;
            case "midi_port":
                code = 0x21;
                port = int.Parse(args[0]);
                is_meta = true;
                break;
            case "end_of_track":
                code = 0x2f;
                is_meta = true;
                break;
            case "set_tempo":
                code = 0x51;
                tempo = int.Parse(args[0]);
                is_meta = true;
                break;
            case "time_signature":
                is_meta = true;
                code = 0x58;
                numerator = int.Parse(args[0]); //4
                denominator = int.Parse(args[1]); //4
                clocks_per_click = int.Parse(args[2]); //24
                notated_32nd_notes_per_beat = int.Parse(args[3]); //8
                break;
            case "key_signature":
                is_meta = true;
                code = 0x59;
                key = int.Parse(args[0]);
                is_major = args[1].Equals("0"); //"0" or "1"
                break;
            //Normal Messages
            case "note_off":
                code = 0x80;
                channel = int.Parse(args[0]);
                note = int.Parse(args[1]);
                velocity = int.Parse(args[2]);
                break;
            case "note_on":
                code = 0x90;
                channel = int.Parse(args[0]);
                note = int.Parse(args[1]);
                velocity = int.Parse(args[2]);
                break;
            case "polytouch":
                code = 0xa0;
                channel = int.Parse(args[0]);
                note = int.Parse(args[1]);
                value = int.Parse(args[2]);
                break;
            case "control_change":
                code = 0xb0;
                channel = int.Parse(args[0]);
                control = int.Parse(args[1]);
                value = int.Parse(args[2]);
                break;
            case "program_change":
                code = 0xc0;
                channel = int.Parse(args[0]);
                program = int.Parse(args[1]);
                break;
            case "aftertouch":
                code = 0xd0;
                channel = int.Parse(args[0]);
                value = int.Parse(args[1]);
                break;
            case "pitchwheel":
                code = 0xe0;
                channel = int.Parse(args[0]);
                pitch = int.Parse(args[1]);
                break;
        }


        if (!type.Equals("sysex")) return;

        code = 0xf0;
        this.data = data;
    }

    public List<byte> createBytes()
    {
        List<byte> data;
        data = is_meta ? createMetaBytes() : createMessageBytes();

        return data;
    }

    public List<byte> createMetaBytes()
    {
        var data = new List<byte>();

        switch (type)
        {
            case "sequence_number":
                data.Add((byte)(number >> 8));
                data.Add((byte)(number & 0xff));
                break;
            case "text":
            case "copyright":
            case "lyrics":
            case "marker":
            case "cue_marker":
            {
                text ??= "";
                data.AddRange(MidiExport.ascii.GetBytes(text));
                break;
            }
            case "track_name":
            case "instrument_name":
            case "device_name":
                data.AddRange(MidiExport.ascii.GetBytes(name));
                break;
            case "channel_prefix":
                data.Add((byte)channel);
                break;
            case "midi_port":
                data.Add((byte)port);
                break;
            case "set_tempo":
                //return [tempo >> 16, tempo >> 8 & 0xff, tempo & 0xff]
                data.Add((byte)(tempo >> 16));
                data.Add((byte)((tempo >> 8) & 0xff));
                data.Add((byte)(tempo & 0xff));
                break;
            case "time_signature":
                data.Add((byte)numerator);
                data.Add((byte)Math.Log(denominator, 2));
                data.Add((byte)clocks_per_click);
                data.Add((byte)notated_32nd_notes_per_beat);
                break;
            case "key_signature":
                data.Add((byte)(key & 0xff));
                data.Add(is_major ? (byte)0x00 : (byte)0x01);
                break;
        }

        var dataLength = data.Count;
        data.InsertRange(0, MidiExport.encodeVariableInt(dataLength));
        data.Insert(0, code);
        data.Insert(0, 0xff);

        return data;
    }


    public List<byte> createMessageBytes()
    {
        var data = new List<byte>();
        switch (type)
        {
            /* if (type.Equals("note_off")) { code = 0x80; channel = int.Parse(args[0]); note = int.Parse(args[1]); velocity = int.Parse(args[2]); }
        if (type.Equals("note_on")) { code = 0x90; channel = int.Parse(args[0]); note = int.Parse(args[1]); velocity = int.Parse(args[2]); }
        if (type.Equals("polytouch")) { code = 0xa0; channel = int.Parse(args[0]); note = int.Parse(args[1]); value = int.Parse(args[2]); }
        if (type.Equals("control_change")) { code = 0xb0; channel = int.Parse(args[0]); control = int.Parse(args[1]); value = int.Parse(args[2]); }
        if (type.Equals("program_change")) { code = 0xc0; channel = int.Parse(args[0]); program = int.Parse(args[1]); }
        if (type.Equals("aftertouch")) { code = 0xd0; channel = int.Parse(args[0]); value = int.Parse(args[1]); }
        if (type.Equals("pitchwheel")) { code = 0xe0; channel = int.Parse(args[0]); pitch = int.Parse(args[1]); }
        if (type.Equals("sysex")) { code = 0xf0; this.data = data; }
          
         */
            case "note_off":
            case "note_on":
                data.Add((byte)(code | (byte)channel));
                data.Add((byte)note);
                data.Add((byte)velocity);
                break;
            case "polytouch":
                data.Add((byte)(code | (byte)channel));
                data.Add((byte)note);
                data.Add((byte)value);
                break;
            case "control_change":
                data.Add((byte)(code | (byte)channel));
                data.Add((byte)control);
                data.Add((byte)value);
                break;
            case "program_change":
                data.Add((byte)(code | (byte)channel));
                data.Add((byte)program);
                break;
            case "aftertouch":
                data.Add((byte)(code | (byte)channel));
                data.Add((byte)value);
                break;
            //14 bit signed integer
            case "pitchwheel":
                data.Add((byte)(code | (byte)channel));
                //data.Add((byte)pitch);
                pitch -= -8192;
                data.Add((byte)(pitch & 0x7f));
                data.Add((byte)(pitch >> 7));
                break;
            case "sysex":
                data.AddRange(this.data);
                break;
        }


        return data;
    }
}