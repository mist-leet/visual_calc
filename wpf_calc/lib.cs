using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IExpression
{
    int Calc();

}

class Expression : IExpression
{
    private char[] valid_bin_ops = { '+', '-', '*', '/' };

    private IExpression[] exps;

    private int size = 0;

    private bool parsed;

    public bool Parsed
    {
        get
        {
            return parsed;
        }
    }

    public int Calc()
    {
        if (!parsed)
            return 0;
        int result = 0;
        foreach (IExpression e in exps)
        {
            result += e.Calc();
        }
        return result;
    }

    public Expression(string s)
    {
        parsed = true;
        s = DeleteAllSpaces(s);
        s = DeleteExternalBrackets(s);
        ParseExpression(s);
    }

    public Expression(IExpression[] es)
    {
        Array.Resize<IExpression>(ref exps, es.Length);
        for (int i = 0; i < es.Length; i++)
            exps[i] = es[i];
    }

    public Expression(IExpression e)
    {

        Array.Resize<IExpression>(ref exps, 1);
        exps[0] = e;
    }

    public void ParseExpression(string s)
    {
        try
        {
            if (!IsValue(s))
            {
                foreach (char op in valid_bin_ops)
                {
                    int i = FindSym(s, op);
                    if (i != -1)
                    {
                        AddExpression(s, i, ref exps, ref size);
                        return;
                    }
                }
            }
            else
            {
                AddExpression(s, ref exps, ref size);
            }
        }
        catch (IndexOutOfRangeException e)
        {
            parsed = false;
            return;

        }
    }

    public override string ToString()
    {
        if (parsed)
            return exps[0].ToString();
        else
            return "unparsed";
    }

    public bool IsValueExp()
    {
        if (parsed && exps.Length == 1 && exps[0] is Value)
            return true;
        else
            return false;
    }

    // Parse methods
    private bool IsValue(string s)
    {
        foreach (char c in s)
            foreach (char op in valid_bin_ops)
                if (c == op)
                    return false;
        return true;
    }

    private void AddExpression(string s, int i, ref IExpression[] exps, ref int size)
    {
        if (i == 0)
            if (s[i] == '-')
            {
                size++;
                Array.Resize<IExpression>(ref exps, size);
                exps[size - 1] = new Substruction(new Expression("0"),
                                                  new Expression(s.Substring(1, s.Length - 1)));
                return;
            }
            else if (s[i] == '+')
            {
                size++;
                Array.Resize<IExpression>(ref exps, size);
                exps[size - 1] = new Expression(s.Substring(1, s.Length - 1));
            }
        
        
        Expression exp1 = new Expression(s.Substring(0, i));
        Expression exp2 = new Expression(s.Substring(i + 1, s.Length - i - 1));

        size++;
        Array.Resize<IExpression>(ref exps, size);

        switch (s[i])
        {
            case '+':
                {
                    exps[size - 1] = new Addition(exp1, exp2);
                    break;
                }
            case '-':
                {
                    exps[size - 1] = new Substruction(exp1, exp2);
                    break;
                }
            case '*':
                {
                    exps[size - 1] = new Multiply(exp1, exp2);
                    break;
                }
            case '/':
                {
                    exps[size - 1] = new Division(exp1, exp2);
                    break;
                }
            default:
                break;
        }
    }

    private void AddExpression(string s, ref IExpression[] exps, ref int size)
    {
        s = DeleteAllBrackets(s);
        size++;
        Array.Resize<IExpression>(ref exps, size);
        exps[size - 1] = new Value(Int32.Parse(s));
    }

    // String methods
    private string DeleteAllBrackets(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '(' || s[i] == ')')
                s = s.Remove(i, 1);
        }
        return s;
    }

    private string DeleteAllSpaces(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == ' ')
                s = s.Remove(i, 1);
        }
        return s;
    }

    private string DeleteExternalBrackets(string s)
    {
        while (s[0] == '(' && s[s.Length - 1] == ')')
        {
            s = s.Remove(0, 1);
            s = s.Remove(s.Length - 1, 1);
        }
        return s;
    }

    private int SkipBrackets(string s, int index)
    {
        int i = index;
        while (s[i] != ')')
        {
            i++;
            if (s[i] == ')')
                return i + 1;
            else if (s[i] == '(')
                i = SkipBrackets(s, i + 1);
        }
        return 0;
    }

    private int FindSym(string s, char op)
    {
        int i;
        for (i = 0; i < s.Length; i++)
            if (s[i] == op)
                return i;
            else if (s[i] == '(')
                i = SkipBrackets(s, i);
        return -1;
    }

    public static string AddExternalBrackets(string s)
    {
        if (!(s[0] == '(' && s[s.Length - 1] == ')'))
        {
            s = "(" + s + ")";
        }
        return s;
    }
}

class Value : IExpression
{
    int value;

    public int Calc()
    {
        return value;
    }

    public Value(int a)
    {
        value = a;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}

class Addition : IExpression
{
    IExpression exp1;
    IExpression exp2;

    public int Calc()
    {
        return exp1.Calc() + exp2.Calc();
    }

    public Addition(IExpression e1, IExpression e2)
    {
        exp1 = e1;
        exp2 = e2;
    }

    public override string ToString()
    {
        return Expression.AddExternalBrackets(exp1.ToString() + " + " + exp2.ToString());
    }
}

class Substruction : IExpression
{
    IExpression exp1;
    IExpression exp2;

    public int Calc()
    {
        return exp1.Calc() - exp2.Calc();
    }

    public Substruction(IExpression e1, IExpression e2)
    {
        exp1 = e1;
        exp2 = e2;

        if (!(((Expression)exp2).IsValueExp()) && ((Expression)exp2).Parsed)
            exp2 = Negative(exp2);
    }

    public override string ToString()
    {
        return Expression.AddExternalBrackets(exp1.ToString() + " - " + exp2.ToString());
    }

    public IExpression Negative(IExpression exp)
    {
        IExpression e = new Expression("0");
        
        return new Expression(new Substruction(e, exp));
    }
}

class Multiply : IExpression
{
    IExpression exp1;
    IExpression exp2;

    public int Calc()
    {
        return exp1.Calc() * exp2.Calc();
    }

    public Multiply(IExpression e1, IExpression e2)
    {
        exp1 = e1;
        exp2 = e2;
    }

    public override string ToString()
    {
        return Expression.AddExternalBrackets(exp1.ToString() + " * " + exp2.ToString());
    }
}

class Division : IExpression
{
    IExpression exp1;
    IExpression exp2;

    public int Calc()
    {
        return exp1.Calc() / exp2.Calc();
    }

    public Division(IExpression e1, IExpression e2)
    {
        exp1 = e1;
        exp2 = e2;
    }

    public override string ToString()
    {
        return Expression.AddExternalBrackets(exp1.ToString() + " / " + exp2.ToString());
    }
}

