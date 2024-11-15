using System.Linq.Expressions;

namespace Ma.ExtensionMethods.Models.Expressions
{
    /// <summary>
    /// Class to help to replace parameter on Expressions
    /// to be able to join them with "and" or "or".
    /// </summary>
    /// <remarks>
    /// Written by Adil Mammadov according to answer at
    /// StackOverflow: http://stackoverflow.com/a/457328/1380428.
    /// </remarks>
    public class ExpressionParameterReplacer
        : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ExpressionParameterReplacer(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}
