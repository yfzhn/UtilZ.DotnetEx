using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.Basepression
{
    /// <summary>
    /// 表达式模型
    /// </summary>
    public class ExpressionNode
    {
        private ExpressionNode _parent = null;

        /// <summary>
        /// 父级节点
        /// </summary>
        public ExpressionNode Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        private ExpressionNode _left = null;
        /// <summary>
        /// 左节点
        /// </summary>
        public ExpressionNode Left
        {
            get { return _left; }
            set { _left = value; }
        }

        private ExpressionNode _right = null;
        /// <summary>
        /// 右节点
        /// </summary>
        public ExpressionNode Right
        {
            get { return _right; }
            set { _right = value; }
        }

        /// <summary>
        /// 是否是叶子节点
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                if (_left == null && _right == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private double _value = default(double);
        /// <summary>
        /// 当前节点的值
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private char _operator = default(char);
        /// <summary>
        /// 非叶子节点时当前表达式的运算符
        /// </summary>
        public char Operator
        {
            get { return _operator; }
            set { _operator = value; }
        }
    }

    /// <summary>
    /// 运算符符号定义类
    /// </summary>
    public class Operators
    {
        /// <summary>
        /// +
        /// </summary>
        public const char Add = '+';

        /// <summary>
        /// 减号[-]
        /// </summary>
        public const char Subtraction = '-';

        /// <summary>
        /// 乘号[*]
        /// </summary>
        public const char Multiply = '*';

        /// <summary>
        /// 除号[/]
        /// </summary>
        public const char Division = '/';

        /// <summary>
        /// 左括号[(]
        /// </summary>
        public const char LeftBrackets = '(';

        /// <summary>
        /// 右括号[)]
        /// </summary>
        public const char RightBrackets = ')';
    }
}
