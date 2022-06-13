using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot;

public enum HotToken
{
    EOF = -1,

    Number,
    String,
    Identifier,

    KeywordMod,
    KeywordUse,
    KeywordLet,
    KeywordFn,
    KeywordRet,
    KeywordIf,
    KeywordElse,
    KeywordTrue,
    KeywordFalse,
    
    SignPlus, // +
    SignMinus, // -
    SignStar, // *
    SignSlash, // /
    SignEqual, // =
    SignDot, // .
    SignComma, // ,
    SignColon, // :
    SignSemicolon, // ;
    SignParentheseLeft, // (
    SignParentheseRight, // )
    SignBraceLeft, // {
    SignBraceRight, // }
    SignBracketLeft, // [
    SignBracketRight, // ]
    SignLess, // <
    SignLessEqual, // <=
    SignGreater, // >
    SignGreaterEqual, // >=
    SignArrowLeft, // <-
    SignArrowRight, // ->
}
