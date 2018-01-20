using Kontur.ImageTransformer.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kontur.ImageTransformer.Tests.Collections
{
    [CollectionDefinition("REST Collection")]
    public class RestCollection : ICollectionFixture<ServerFixture>
    {
    }
}
