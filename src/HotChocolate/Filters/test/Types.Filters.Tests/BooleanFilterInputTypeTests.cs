using System.Threading.Tasks;
using HotChocolate.Language;
using HotChocolate.Tests;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;
using static HotChocolate.Tests.TestHelper;

namespace HotChocolate.Types.Filters
{
    public class BooleanFilterInputTypeTests
    {
        [Fact]
        public async Task Create_Filter_Discover_Everything_Implicitly()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(new FooFilterTypeDefaults());

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Create_Filter_Discover_Operators_Implicitly()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(new FooFilterType());

            // assert
            schema.ToString().MatchSnapshot();
        }

        /// <summary>
        /// This test checks if the binding of all allow methods are correct
        /// </summary>
        [Fact]
        public async Task Create_Filter_Declare_Operators_Explicitly()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(new FilterInputType<Foo>(d =>
            {
                d.Filter(x => x.Bar)
                    .BindFiltersExplicitly()
                    .AllowEquals().And()
                    .AllowNotEquals();
            }));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Declare_Name_Explicitly()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(new FilterInputType<Foo>(descriptor =>
            {
                descriptor.Filter(x => x.Bar)
                    .BindFiltersExplicitly()
                    .AllowEquals()
                    .Name("custom_equals");
            }));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Declare_Description_Explicitly()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(new FilterInputType<Foo>(descriptor =>
            {
                descriptor.Filter(x => x.Bar)
                    .BindFiltersExplicitly()
                    .AllowEquals()
                    .Description("custom_equals_description");
            }));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Declare_Directive_By_Name()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(builder =>
                builder.AddType(new FilterInputType<Foo>(d =>
                {
                    d.Filter(x => x.Bar)
                        .BindFiltersExplicitly()
                        .AllowEquals()
                        .Directive("bar");
                }))
                .AddDirectiveType(new DirectiveType(d => d
                    .Name("bar")
                    .Location(DirectiveLocation.InputFieldDefinition))));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Declare_Directive_By_Name_With_Argument()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(builder =>
                builder.AddType(new FilterInputType<Foo>(d =>
                {
                    d.Filter(x => x.Bar)
                        .BindFiltersExplicitly()
                        .AllowEquals()
                        .Directive("bar",
                            new ArgumentNode("qux",
                                new StringValueNode("foo")));
                }))
                .AddDirectiveType(new DirectiveType(d => d
                    .Name("bar")
                    .Location(DirectiveLocation.InputFieldDefinition)
                    .Argument("qux")
                    .Type<StringType>())));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Declare_Directive_With_Clr_Type()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(builder =>
                builder.AddType(new FilterInputType<Foo>(d =>
                {
                    d.Filter(x => x.Bar)
                        .BindFiltersExplicitly()
                        .AllowEquals()
                        .Directive<Bar>();
                }))
                .AddDirectiveType(new DirectiveType<Bar>(d => d
                    .Location(DirectiveLocation.InputFieldDefinition))));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Declare_Directive_With_Clr_Instance()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(builder =>
                builder.AddType(new FilterInputType<Foo>(d =>
                {
                    d.Filter(x => x.Bar)
                        .BindFiltersExplicitly()
                        .AllowEquals()
                        .Directive(new Bar());
                }))
                .AddDirectiveType(new DirectiveType<Bar>(d => d
                    .Location(DirectiveLocation.InputFieldDefinition))));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Bind_Filter_Implicitly()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(
                new FilterInputType<Foo>(descriptor =>
                {
                    descriptor
                        .BindFieldsExplicitly()
                        .Filter(x => x.Bar)
                        .BindFiltersExplicitly()
                        .BindFiltersImplicitly();
                }));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Ignore_Field_Fields()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(
                new FilterInputType<Foo>(d => d
                    .Ignore(f => f.Bar)));

            // assert
            schema.ToString().MatchSnapshot();
        }

        [Fact]
        public async Task Ignore_Field_2()
        {
            // arrange
            // act
            ISchema schema = await CreateSchemaAsync(
                new FilterInputType<Foo>(d => d
                    .Filter(f => f.Bar)
                    .Ignore()));

            // assert
            schema.ToString().MatchSnapshot();
        }

        public class Foo
        {
            public bool Bar { get; set; }
        }

        public class Bar
        {
            public string Qux { get; set; }
        }

        public class FooFilterType
            : FilterInputType<Foo>
        {
            protected override void Configure(
                IFilterInputTypeDescriptor<Foo> descriptor)
            {
                descriptor.Filter(x => x.Bar);
            }
        }

        public class FooFilterTypeDefaults
            : FilterInputType<Foo>
        {
        }
    }
}
