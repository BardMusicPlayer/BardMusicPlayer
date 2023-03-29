/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Script.BasicSharp;

public enum Token
{
    Unknown,

    Identifier,
    Value,

    //Keywords
    Print,
    Macro,
    If,
    EndIf,
    Then,
    Else,
    For,
    To,
    Next,
    Step,
    Goto,
    Input,
    Playtime,
    Let,
    Gosub,
    Return,
    Rem,
    End,
    Assert,
    Select,
    UnSelect,
    Sleep,
    TapKey,
    CPrint,

    NewLine,
    Colon,
    Semicolon,
    Comma,

    Plus,
    Minus,
    Slash,
    Asterisk,
    Caret,
    Equal,
    Less,
    More,
    NotEqual,
    LessEqual,
    MoreEqual,
    Or,
    And,
    Not,

    LParen,
    RParen,

    EOF = -1 //End Of File
}