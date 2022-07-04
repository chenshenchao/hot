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

    KeywordTrue,
    KeywordFalse,
    KeywordMod,
    KeywordUse,
    KeywordLet,
    KeywordFn,
    KeywordRet,
    KeywordIf,
    KeywordElse,
    KeywordLoop,
    KeywordIn,
    KeywordBreak,
    
    SignPlus, // +
    SignMinus, // -
    SignStar, // *
    SignSlash, // /
    SignAmpersand, // &
    SignVertical, // |
    SignTilde, // ~
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
    SignExclamation, // !
    SignAnd, // &&
    SignOr, // ||
    SignArrowLeft, // <-
    SignArrowRight, // ->
}
