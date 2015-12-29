using Pegasus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP4VS.ConfigNodeServices
{
    public partial class InProgressNodeParser
    {
        private struct TokenStart
        {
            public int location;
        }
        private void PushTokenStart(Cursor state)
        {
            var startStack = state["Start"] as Stack<TokenStart>;
            if (startStack == null)
                state["Start"] = startStack = new Stack<TokenStart>();
            startStack.Push(new TokenStart { location = state.Location });
        }
        private void MarkTokenEnd(Cursor state, Token.Type type)
        {
            var startStack = state["Start"] as Stack<TokenStart>;
            if (startStack == null)
                throw new InvalidOperationException("Cannot end a token when none have started");
            var startMark = startStack.Pop();
            var tokenList = state["List"] as List<Token>;
            if (tokenList == null)
                state["List"] = tokenList = new List<Token>();
            tokenList.Add(new Token { Location = startMark.location, Length = state.Location - startMark.location, TokenType = type });
        }
    }
}
