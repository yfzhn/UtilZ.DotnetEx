using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.Basepression
{
    /// <summary>
    /// 字符串表达式计算类
    /// </summary>
    public class ExpressionCalculator
    {
        /// <summary>
        /// 字符串表达式计算
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public double ExpressonCalculate(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new Exception("表达式不能为空或null");
            }
            expression = expression.Replace(" ", string.Empty);

            if (expression[0] == Operators.Subtraction)
            {
                expression = "0" + expression;
            }

            if (!ExpressionValidate(expression))
            {
                throw new Exception(string.Format("表达式{0}不是有效的表达式,表达式中不能含有非数字或是四则运算符及小括号运算符", expression));
            }
            if (BaseCalate.IsOperator(expression[expression.Length - 1]))
            {
                throw new Exception(string.Format("表达式{0}不是有效的表达式,表达式结尾不能为+-*/四种运算符", expression));
            }
            //return this.Calculator(this.BaseExpressionParse(expression));//测试该函数用
            //return this.Calculator(this.BlendExpressionParse(expression));//测试该函数用

            return this.Calculator(this.ExpressionParse(expression));
        }

        /// <summary>
        /// 表达式验证
        /// </summary>
        /// <param name="expression">需要验证的表达式</param>
        /// <returns>验证通过返回true,非法表达式返回false</returns>
        private bool ExpressionValidate(string expression)
        {
            int leftBrackets = 0;
            int rightBrackets = 0;

            for (int i = 0; i < expression.Length; i++)
            {
                //左括号计数
                if (expression[i] == Operators.LeftBrackets)
                {
                    leftBrackets++;
                }
                //右括号计数
                if (expression[i] == Operators.RightBrackets)
                {
                    rightBrackets++;
                }
                //如果是数学运算符则跳过
                if (BaseCalate.IsMathOperator(expression[i]))
                {
                    continue;
                }

                //如果不是数学运算符,也不是数字,则返回false,验证不通过
                if (expression[i] < '0' || expression[i] > '9')
                {
                    return false;
                }
            }

            //左括号和右括号数目不一样,则返回false,验证不通过
            if (leftBrackets != rightBrackets)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 仅加法和减法或仅乘法和除法(不能有加减和乘除混合)表达式解析生成树
        /// </summary>
        /// <param name="expression">要计算的表达式字符串</param>
        /// <returns>表达式节点树</returns>
        private ExpressionNode BaseExpressionParse(string expression)
        {
            ExpressionNode root = new ExpressionNode();
            int valueStaratIndex = 0;
            string value = string.Empty;
            char chDefaultValue = default(char);
            char currentOperator = chDefaultValue;

            for (int i = 0; i < expression.Length; i++)
            {
                if (BaseCalate.IsOperator(expression[i]))
                {
                    value = expression.Substring(valueStaratIndex, i - valueStaratIndex);
                    valueStaratIndex = i + 1;

                    currentOperator = expression[i];
                    continue;
                }
                else
                {
                    if (!string.IsNullOrEmpty(value) && currentOperator != chDefaultValue)
                    {
                        if (root.Left == null)
                        {
                            var lefttLeafNode = new ExpressionNode();
                            lefttLeafNode.Value = double.Parse(value.Trim());
                            lefttLeafNode.Parent = root;

                            root.Operator = currentOperator;
                            root.Left = lefttLeafNode;
                        }
                        else if (root.Left != null && root.Right == null)
                        {
                            var rightLeafNode = new ExpressionNode();
                            rightLeafNode.Value = double.Parse(value.Trim());
                            rightLeafNode.Parent = root;

                            root.Right = rightLeafNode;

                            var parentNode = new ExpressionNode();
                            parentNode.Operator = currentOperator;
                            parentNode.Left = root;
                            root.Parent = parentNode;
                            root = parentNode;
                        }

                        value = string.Empty;
                    }
                }
            }

            value = expression.Substring(valueStaratIndex, expression.Length - valueStaratIndex);
            if (root.Left != null)
            {
                var endLeaf = new ExpressionNode();
                endLeaf.Value = double.Parse(value.Trim());
                endLeaf.Parent = root;
                root.Right = endLeaf;
            }
            else
            {
                root.Value = double.Parse(value.Trim());
            }
            return root;
        }

        /// <summary>
        /// 无括号的加减乘除混合表达式运算
        /// </summary>
        /// <param name="expression">要计算的表达式字符串</param>
        /// <returns>表达式节点树</returns>
        private ExpressionNode BlendExpressionParse(string expression)
        {
            int startSubExpressionIndex = 0;
            int endSubExpressionIndex = -1;
            string subExpression = string.Empty;
            double subExpressionValue = 0.0d;

            if (expression[0] == Operators.Subtraction)
            {
                expression = "0" + expression;
            }

            while (true)
            {
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == Operators.Multiply || expression[i] == Operators.Division)
                    {
                        if (i + 1 == expression.Length)
                        {
                            endSubExpressionIndex = i + 1;
                        }
                        else
                        {
                            for (int j = i + 1; j < expression.Length; j++)
                            {
                                if (BaseCalate.IsOperator(expression[j]))
                                {
                                    endSubExpressionIndex = j;
                                    break; ;
                                }
                            }
                            if (endSubExpressionIndex == -1)
                            {
                                endSubExpressionIndex = expression.Length;
                            }
                        }

                        subExpression = expression.Substring(startSubExpressionIndex, endSubExpressionIndex - startSubExpressionIndex);
                        subExpressionValue = BaseCalate.BaseCal(subExpression);
                        expression = expression.Remove(startSubExpressionIndex, endSubExpressionIndex - startSubExpressionIndex);
                        expression = expression.Insert(startSubExpressionIndex, subExpressionValue.ToString());

                        endSubExpressionIndex = -1;
                        break;
                    }
                    if (expression[i] == Operators.Add || expression[i] == Operators.Subtraction)
                    {
                        startSubExpressionIndex = i + 1;
                    }
                }

                if (!expression.Contains(Operators.Multiply) && !expression.Contains(Operators.Division))
                {
                    break;
                }
            }

            ExpressionNode root = BaseExpressionParse(expression);
            return root;
        }

        /// <summary>
        /// 解析通用表达式
        /// </summary>
        /// <param name="expression">表达式字符串</param>
        /// <returns>表达式结点树</returns>
        private ExpressionNode ExpressionParse(string expression)
        {
            int leftBracketsIndex = 0;
            int rightBracketsIndex = -1;
            string subExpression = string.Empty;
            string subExpressionValue = string.Empty;

            while (true)
            {
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == Operators.LeftBrackets)
                    {
                        leftBracketsIndex = i;
                        continue;
                    }
                    if (expression[i] == Operators.RightBrackets)
                    {
                        rightBracketsIndex = i;
                        subExpression = expression.Substring(leftBracketsIndex + 1, rightBracketsIndex - leftBracketsIndex - 1);
                        subExpressionValue = this.Calculator(this.BlendExpressionParse(subExpression)).ToString();
                        expression = expression.Remove(leftBracketsIndex + 1, rightBracketsIndex - leftBracketsIndex - 1);
                        expression = expression.Insert(leftBracketsIndex + 1, subExpressionValue);

                        if (expression[leftBracketsIndex] == Operators.LeftBrackets &&
                            expression[leftBracketsIndex + subExpressionValue.Length + 1] == Operators.RightBrackets)
                        {
                            if (subExpressionValue[0] == Operators.Subtraction)
                            {
                                if (leftBracketsIndex == 0)
                                {
                                    expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                    expression = expression.Remove(leftBracketsIndex, 1);
                                }
                                else if (leftBracketsIndex > 0)
                                {
                                    switch (expression[leftBracketsIndex - 1])
                                    {
                                        case Operators.Add:
                                            //3-(2+(-2)*4+5)+25
                                            expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                            expression = expression.Remove(leftBracketsIndex, 2);
                                            expression = expression.Remove(leftBracketsIndex - 1, 1);
                                            expression = expression.Insert(leftBracketsIndex - 1, "-");
                                            //3-(2-2*4+5)+25
                                            break;
                                        case Operators.Subtraction:
                                            //3-(2-(-2)*4+5)+25
                                            expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                            expression = expression.Remove(leftBracketsIndex, 2);
                                            expression = expression.Remove(leftBracketsIndex - 1, 1);
                                            expression = expression.Insert(leftBracketsIndex - 1, "+");
                                            //3-(2+2*4+5)+25
                                            break;
                                        case Operators.Multiply:
                                        case Operators.Division:
                                            //1.   3-2*(-2)*4+25
                                            //2.   3+2*(-2)*4+25
                                            //3.   6.5/(-2)*0.5
                                            bool isProcessed = false;
                                            for (int j = leftBracketsIndex - 1 - 1; j >= 0; j--)
                                            {
                                                if (expression[j] == Operators.LeftBrackets)
                                                {
                                                    expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                                    expression = expression.Remove(leftBracketsIndex, 2);
                                                    expression = expression.Insert(j + 1, "-");
                                                    isProcessed = true;
                                                    break;
                                                }
                                                if (expression[j] == Operators.Add)
                                                {
                                                    expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                                    expression = expression.Remove(leftBracketsIndex, 2);
                                                    expression = expression.Remove(j, 1);
                                                    expression = expression.Insert(j, "-");
                                                    isProcessed = true;
                                                    break;
                                                    //3-2*2*4+25
                                                }
                                                else if (expression[j] == Operators.Subtraction)
                                                {
                                                    expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                                    expression = expression.Remove(leftBracketsIndex, 2);
                                                    expression = expression.Remove(j, 1);
                                                    expression = expression.Insert(j, "+");
                                                    isProcessed = true;
                                                    break;
                                                    //3+2*2*4+25
                                                }
                                            }
                                            if (!isProcessed)
                                            {
                                                expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                                expression = expression.Remove(leftBracketsIndex, 2);
                                                if (expression[0] == Operators.Subtraction)
                                                {
                                                    expression = expression.Remove(0, 1);
                                                }
                                                else
                                                {
                                                    expression = expression.Insert(0, "-");
                                                }
                                            }
                                            //1.   3+2*2*4+25
                                            //2.   3-2*2*4+25
                                            //3.   -6.5/2*0.5
                                            break;
                                        case Operators.LeftBrackets:
                                            //3-((-2)*4+5)+25
                                            expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                            expression = expression.Remove(leftBracketsIndex, 1);
                                            //3-(-2*4+5)+25
                                            break;
                                        default:
                                            throw new Exception("未知的运算符:" + expression[leftBracketsIndex - 1 - 1].ToString());
                                    }
                                }
                                else
                                {
                                    throw new Exception("未知索引");
                                }
                            }
                            else
                            {
                                if (leftBracketsIndex == 0)
                                {
                                    expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                    expression = expression.Remove(leftBracketsIndex, 1);
                                }
                                else if (leftBracketsIndex > 0)
                                {
                                    switch (expression[leftBracketsIndex - 1])
                                    {
                                        case Operators.Add:
                                        case Operators.Subtraction:
                                        case Operators.Multiply:
                                        case Operators.Division:
                                            expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                            expression = expression.Remove(leftBracketsIndex, 1);
                                            break;
                                        case Operators.LeftBrackets:
                                            expression = expression.Remove(leftBracketsIndex + subExpressionValue.Length + 1, 1);
                                            expression = expression.Remove(leftBracketsIndex - 1, 1);
                                            break;
                                        default:
                                            throw new Exception("未知的运算符:" + expression[leftBracketsIndex - 1 - 1].ToString());

                                    }
                                }
                                else
                                {
                                    throw new Exception("未知索引");
                                }
                            }
                        }
                        break;
                    }
                }

                if (!expression.Contains(Operators.LeftBrackets) && !expression.Contains(Operators.RightBrackets))
                {
                    break;
                }
            }

            ExpressionNode root = this.BlendExpressionParse(expression);
            return root;
        }

        /// <summary>
        /// 树值计算
        /// </summary>
        /// <param name="root">要计算的节点</param>
        /// <returns>节点计算结果值</returns>
        private double Calculator(ExpressionNode root)
        {
            double result = default(double);
            double leftValue = default(double);
            double rightValue = default(double);

            if (root == null)
            {
                return result;
            }

            if (root.Left != null)
            {
                if (!root.Left.IsLeaf)
                {
                    leftValue = Calculator(root.Left);
                }
                else
                {
                    leftValue = root.Left.Value;
                }
            }
            if (root.Right != null)
            {
                if (!root.Right.IsLeaf)
                {
                    rightValue = Calculator(root.Right);
                }
                else
                {
                    rightValue = root.Right.Value;
                }
            }

            if (!BaseCalate.IsOperator(root.Operator))
            {
                result = root.Value;
            }
            else
            {
                result = BaseCalate.BaseCal(leftValue, rightValue, root.Operator.ToString());
            }

            return result;
        }
    }
}
