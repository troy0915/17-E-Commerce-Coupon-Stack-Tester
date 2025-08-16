using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum CouponType
{
    FixedAmount,   
    Percentage,    
    FreeShipping   
}

public class Coupon
{
    public string Code { get; }
    public CouponType Type { get; }
    public double Value { get; } 
    public HashSet<string> ExclusiveWith { get; } = new HashSet<string>();

    public Coupon(string code)
    {
        Code = code.ToUpper();

        if (Code.EndsWith("OFF") && double.TryParse(Code.Replace("OFF", ""), out double fixedAmt))
        {
            Type = CouponType.FixedAmount;
            Value = fixedAmt;
        }
        else if (Code.EndsWith("%") && double.TryParse(Code.Replace("%", ""), out double percent))
        {
            Type = CouponType.Percentage;
            Value = percent;
        }
        else if (Code == "FREESHIP")
        {
            Type = CouponType.FreeShipping;
            Value = 0;
        }
        else
        {
            throw new ArgumentException($"Invalid coupon format: {code}");
        }
    }

    public double Apply(double subtotal, double shippingCost)
    {
        switch (Type)
        {
            case CouponType.FixedAmount:
                return Math.Max(0, subtotal - Value);
            case CouponType.Percentage:
                return subtotal * (1 - Value / 100.0);
            case CouponType.FreeShipping:
                return subtotal; 
            default:
                return subtotal;
        }
    }
}
public class Program
{
    private static double shippingCost = 5.00;
namespace _17__E_Commerce_Coupon_Stack_Tester
{
    internal class Program
    {
        private static double shippingCost;

        static void Main(string[] args)
        {
            double cartTotal = 100.0;

            // Create coupons
            var coupons = new List<Coupon>
        {
            new Coupon("10OFF"),
            new Coupon("15%"),
            new Coupon("FREESHIP")
        };

            // Define mutual exclusions (example: "10OFF" and "15%" can't be combined)
            coupons.First(c => c.Code == "10OFF").ExclusiveWith.Add("15%");
            coupons.First(c => c.Code == "15%").ExclusiveWith.Add("10OFF");

            // Find best combination
            var (bestSet, bestPayable) = FindBestCouponStack(cartTotal, coupons);

            Console.WriteLine("Best coupons to use: " + (bestSet.Count > 0 ? string.Join(", ", bestSet.Select(c => c.Code)) : "None"));
            Console.WriteLine($"Final payable: ${bestPayable:F2}");
            Console.WriteLine($"You saved: ${cartTotal + shippingCost - bestPayable:F2}");
        }

        public static (List<Coupon> bestSet, double bestPayable) FindBestCouponStack(double cartTotal, List<Coupon> coupons)
        {
            var allCombos = GetAllSubsets(coupons);
            double bestPayable = double.MaxValue;
            List<Coupon> bestSet = new List<Coupon>();

            foreach (var combo in allCombos)
            {
                if (!IsValidCombo(combo)) continue;

                double subtotal = cartTotal;
                double shipCost = shippingCost;

                foreach (var coupon in combo)
                {
                    if (coupon.Type == CouponType.FreeShipping)
                        shipCost = 0;
                    else
                        subtotal = coupon.Apply(subtotal, shipCost);
                }

                double totalPayable = subtotal + shipCost;

                if (totalPayable < bestPayable)
                {
                    bestPayable = totalPayable;
                    bestSet = combo;
                }
            }

            return (bestSet, bestPayable);
        }

        public static List<List<Coupon>> GetAllSubsets(List<Coupon> coupons)
        {
            var subsets = new List<List<Coupon>>();
            int total = 1 << coupons.Count; // 2^n combinations

            for (int mask = 0; mask < total; mask++)
            {
                var subset = new List<Coupon>();
                for (int i = 0; i < coupons.Count; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        subset.Add(coupons[i]);
                }
                subsets.Add(subset);
            }
            return subsets;
        }

        public static bool IsValidCombo(List<Coupon> combo)
        {
            foreach (var c in combo)
            {
                foreach (var excl in c.ExclusiveWith)
                {
                    if (combo.Any(x => x.Code == excl))
                        return false;
                }
            }
            return true;
        }
    }
}




