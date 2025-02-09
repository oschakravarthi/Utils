namespace SubhadraSolutions.Utils.Kusto.Shared.Linq;

public enum JoinKind
{
    InnerUnique,
    Inner,
    LeftOuter,
    RightOuter,
    FullOuter,
    LeftAnti,
    RightAnti,
    LeftSemi,
    RightSemi
}