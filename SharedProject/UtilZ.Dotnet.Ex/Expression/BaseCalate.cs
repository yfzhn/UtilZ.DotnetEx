using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.Basepression
{
    /// <summary>
    /// 基础计算类
    /// </summary>
    internal static class BaseCalate
    {
        /// <summary>
        /// 基本的加减乘除运算
        /// </summary>
        /// <param name="left">左操作数字符串</param>
        /// <param name="right">右操作数字符串</param>
        /// <param name="operate">运算符</param>
        /// <returns>运算结果</returns>
        internal static double BaseCal(string left, string right, string operate)
        {
            if (string.IsNullOrEmpty(left))
            {
                throw new OperNumException("左操作数不能为null");
            }

            if (string.IsNullOrEmpty(right))
            {
                throw new OperNumException("右操作数不能为null");
            }

            if (string.IsNullOrEmpty(operate))
            {
                throw new OperNumException("运算符不能为null");
            }

            double result = default(double);
            double leftNum = default(double);
            double rightNum = default(double);

            if (!double.TryParse(left, out leftNum))
            {
                throw new OperNumException("左操作数不不是有效的数值");
            }

            if (!double.TryParse(right, out rightNum))
            {
                throw new OperNumException("右操作数不不是有效的数值");
            }

            operate = operate.Trim();
            switch (operate)
            {
                case "+":
                    result = leftNum + rightNum;
                    break;
                case "-":
                    result = leftNum - rightNum;
                    break;
                case "*":
                    result = leftNum * rightNum;
                    break;
                case "/":
                    if (rightNum == 0)
                    {
                        throw new OperNumException("当为除法运算时右操作数不能为0");
                    }
                    result = leftNum / rightNum;
                    break;
                default:
                    throw new OperNumException("未知的运算符:" + operate);
            }

            return result;
        }

        /// <summary>
        /// 基本的加减乘除运算
        /// </summary>
        /// <param name="leftNum">左操作数</param>
        /// <param name="rightNum">右操作数</param>
        /// <param name="operate">运算符</param>
        /// <returns>运算结果</returns>
        internal static double BaseCal(double leftNum, double rightNum, string operate)
        {
            if (string.IsNullOrEmpty(operate))
            {
                throw new OperNumException("运算符不能为null");
            }

            double result = default(double);
            operate = operate.Trim();

            switch (operate)
            {
                case "+":
                    result = leftNum + rightNum;
                    break;
                case "-":
                    result = leftNum - rightNum;
                    break;
                case "*":
                    result = leftNum * rightNum;
                    break;
                case "/":
                    result = leftNum / rightNum;
                    break;
                default:
                    throw new OperNumException("未知的运算符:" + operate);
            }

            return result;
        }

        /// <summary>
        /// 基本的加减乘除运算
        /// </summary>
        /// <param name="baseExpression">基本表达式[仅由两个操作数和一个运算符的计算]</param>
        /// <returns>运算结果</returns>
        internal static double BaseCal(string baseExpression)
        {
            string leftNumStr = string.Empty;
            string rightNumStr = string.Empty;
            string operate = string.Empty;

            for (int i = 0; i < baseExpression.Length; i++)
            {
                if (IsOperator(baseExpression[i]))
                {
                    leftNumStr = baseExpression.Substring(0, i);
                    rightNumStr = baseExpression.Substring(i + 1, baseExpression.Length - i - 1);
                    operate = baseExpression[i].ToString();
                }
            }

            return BaseCal(leftNumStr, rightNumStr, operate);
        }

        /// <summary>
        /// 是否+-*/运算符
        /// </summary>
        /// <param name="ch">字符</param>
        /// <returns>如果是四则运算符中的一种则返回true,否则返回false</returns>
        internal static bool IsOperator(char ch)
        {
            if (ch == Operators.Add ||
                ch == Operators.Subtraction ||
                ch == Operators.Multiply ||
                ch == Operators.Division)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是数学运算符+-*/().
        /// </summary>
        /// <param name="ch">字符</param>
        /// <returns>如果是数学运算符+-*/.中的一种则返回true,否则返回false</returns>
        internal static bool IsMathOperator(char ch)
        {
            if (ch == Operators.Add ||
                ch == Operators.Subtraction ||
                ch == Operators.Multiply ||
                ch == Operators.Division ||
                ch == '.' ||
                ch == Operators.LeftBrackets ||
                ch == Operators.RightBrackets)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 表达式是否包含某种运算符
        /// </summary>
        /// <param name="expression">简单的表达式[3+2或3*2]</param>
        /// <returns>表达式计算结果</returns>
        private static bool ContainsOperator(string expression)
        {
            if (expression.Contains(Operators.Add) ||
                expression.Contains(Operators.Subtraction) ||
                expression.Contains(Operators.Multiply) ||
                expression.Contains(Operators.Division))
            {
                return true;
            }

            return false;
        }
    }
}
