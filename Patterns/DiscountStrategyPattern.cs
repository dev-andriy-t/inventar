using System;

namespace RetailingApp.Patterns
{

    public interface IDiscountStrategy
    {
        double CalculateDiscountedPrice(double originalPrice);
    }

    public class NoDiscountStrategy : IDiscountStrategy
    {
        public double CalculateDiscountedPrice(double originalPrice)
        {
            return originalPrice;
        }
    }

    public class HalfPriceStrategy : IDiscountStrategy
    {
        public double CalculateDiscountedPrice(double originalPrice)
        {
            return originalPrice / 2;
        }
    }

    public class BlackFridayStategy: IDiscountStrategy
    {
        public double CalculateDiscountedPrice(double originalPrice)
        {
            return originalPrice * 0.7;
        }
    }

    public class PercentStrategy: IDiscountStrategy
    {
        private readonly double _percent;
        public PercentStrategy(double percent){
            _percent = percent;
        }
        public double CalculateDiscountedPrice(double OriginalPrice)
        {
            return OriginalPrice * (1 - _percent);
        }
    }

    public class FixedAmountDiscountStrategy : IDiscountStrategy
    {
        private readonly double _discountAmount;

        public FixedAmountDiscountStrategy(double discountAmount)
        {
            _discountAmount = discountAmount;
        }

        public double CalculateDiscountedPrice(double originalPrice)
        {
            return Math.Max(0, originalPrice - _discountAmount);
        }
    }
}
