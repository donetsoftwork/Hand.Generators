using Hand.Generators;
using Hand.Sources;
using Hand.Transform;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;

namespace Hand;

public class ConvertTransform : IGeneratorTransform<IEnumerable<IGeneratorSource>>
{
    /// <inheritdoc />
    public IEnumerable<IGeneratorSource> Transform(AttributeContext context, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
}
