using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace TestStack.Seleno.Browserstack.UnitTests.Assertions
{
    public class NonGenericDictionaryAssertions : NonGenericCollectionAssertions
    {
        public NonGenericDictionaryAssertions(IDictionary dictionary) : base(dictionary)
        {
        }

        public AndConstraint<NonGenericCollectionAssertions> OnlyContain(object key, object value)
        {
            var dictionary = (IDictionary) Subject;
            Execute
                .Assertion
                .ForCondition(dictionary.Contains(key) && dictionary[key] == value)
                .FailWith("Expected to only contain data {0}, but found {1}.", new KeyValuePair<object,object>( key, value),  dictionary);

            return new AndConstraint<NonGenericCollectionAssertions>(this);
        }
    }
}