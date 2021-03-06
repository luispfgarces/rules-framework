namespace Rules.Framework.Builder
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    internal class ValueConditionNodeBuilder<TConditionType> : IValueConditionNodeBuilder<TConditionType>
    {
        private readonly TConditionType conditionType;

        public ValueConditionNodeBuilder(TConditionType conditionType)
        {
            this.conditionType = conditionType;
        }

        public IValueConditionNodeBuilder<TConditionType, T> OfDataType<T>()
            => new ValueConditionNodeBuilder<TConditionType, T>(this.conditionType);
    }

    internal class ValueConditionNodeBuilder<TConditionType, TDataType> : IValueConditionNodeBuilder<TConditionType, TDataType>
    {
        private readonly TConditionType conditionType;
        private Operators comparisonOperator;
        private object operand;

        public ValueConditionNodeBuilder(TConditionType conditionType)
        {
            this.conditionType = conditionType;
        }

        public IValueConditionNode<TConditionType> Build()
        {
            switch (this.operand)
            {
                case decimal _:
                case IEnumerable<decimal> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Decimal, this.conditionType, this.comparisonOperator, this.operand);

                case int _:
                case IEnumerable<int> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Integer, this.conditionType, this.comparisonOperator, this.operand);

                case bool _:
                case IEnumerable<bool> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.Boolean, this.conditionType, this.comparisonOperator, this.operand);

                case string _:
                case IEnumerable<string> _:
                    return new ValueConditionNode<TConditionType>(DataTypes.String, this.conditionType, this.comparisonOperator, this.operand);

                default:
                    throw new NotSupportedException($"The data type is not supported: {typeof(TDataType).FullName}.");
            }
        }

        public IValueConditionNodeBuilder<TConditionType, TDataType> SetOperand(TDataType value)
        {
            this.operand = value;

            return this;
        }

        public IValueConditionNodeBuilder<TConditionType, TDataType> SetOperand(IEnumerable<TDataType> value)
        {
            this.operand = value;

            return this;
        }

        public IValueConditionNodeBuilder<TConditionType, TDataType> WithComparisonOperator(Operators comparisonOperator)
        {
            this.comparisonOperator = comparisonOperator;

            return this;
        }
    }
}