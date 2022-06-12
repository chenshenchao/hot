using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot;

public enum HotToken
{
    EOF = -1,

    Bool,
    Number,
    String,
    Identifier,

    KeywordLet,
    KeywordFn,
    KeywordRet,
    KeywordIf,
    KeywordElse,
    
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
