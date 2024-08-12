using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HW_Async_Thread
{
    public class Program
    {
        public static async Task Main()
        {
            // 测试用例: (a + b) + (c + d)
            ValueExpr a = new(1);
            ValueExpr b = new(2);
            ValueExpr c = new(3);
            ValueExpr d = new(4);
            AddExpr add1 = new(a, b);
            AddExpr add2 = new(c, d);
            AddExpr add3 = new(add1, add2);
            Console.WriteLine(add3.Val);
            a.NewVal = 5;
            await add3.Update(); 
            Console.WriteLine(add3.Val);
        }
    }

    public abstract class Expr
    {
        protected List<Expr> parents = new List<Expr>();

        public abstract int Val { get; }

        public abstract Task Update();

        public abstract void Register(Expr parent);

        protected void NotifyParents()
        {
            foreach (var parent in parents)
            {
                _ = parent.Update(); 
            }
        }
    }

    public class ValueExpr : Expr
    {
        private int val;
        private bool isDirty = true;

        public override int Val
        {
            get
            {
                if (isDirty)
                {
                    throw new InvalidOperationException("Value is not up-to-date.");
                }
                return val;
            }
        }

        public int NewVal
        {
            set
            {
                if (val != value)
                {
                    val = value;
                    isDirty = true;
                    NotifyParents();
                }
            }
        }

        public ValueExpr(int initVal)
        {
            val = initVal;
            isDirty = false;
        }

        public override async Task Update()
        {
            isDirty = false;
            await Task.CompletedTask;
        }

        public override void Register(Expr parent)
        {
            parents.Add(parent);
        }
    }

    public class AddExpr : Expr
    {
        private int val;
        private bool isDirty = true;
        private readonly Expr exprA;
        private readonly Expr exprB;

        public override int Val
        {
            get
            {
                if (isDirty)
                {
                    throw new InvalidOperationException("Value is not up-to-date.");
                }
                return val;
            }
        }

        public AddExpr(Expr A, Expr B)
        {
            exprA = A;
            exprB = B;
            A.Register(this);
            B.Register(this);
        }

        public override async Task Update()
        {
            val = exprA.Val + exprB.Val;
            isDirty = false;
            await Task.CompletedTask;
        }

        public override void Register(Expr parent)
        {
            parents.Add(parent);
        }
    }
}

