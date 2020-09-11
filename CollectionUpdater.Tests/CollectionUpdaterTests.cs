using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CollectionUpdater.Tests
{
    public class CollectionUpdaterTests
    {
        [Fact]
        public void Should_add_item_from_src_to_dest_on_same_types()
        {
            var srcCol = new List<SimpleObj>
            {
                new SimpleObj { Id = 1, StringValue = "string1" }
            };
            var destCol = new List<SimpleObj>();
            

            destCol.AsCollectionUpdater(srcCol)
                .PairUsing((src, dest) => src.Id == dest.Id)
                .SetMapFunc((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.StringValue = src.StringValue;
                })
                .Update();


            destCol.Should().BeEquivalentTo(srcCol);
        }

        [Fact]
        public void Should_add_item_from_src_to_dest_on_different_types()
        {
            var srcCol = new List<SimpleObj>
            {
                new SimpleObj { Id = 1, StringValue = "string1" }
            };
            var destCol = new List<AnotherSimpleObj>();


            destCol.AsCollectionUpdater(srcCol)
                .PairUsing((src, dest) => src.Id == dest.Id)
                .SetMapFunc((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.StringValue = src.StringValue;
                })
                .Update();


            destCol.Should().BeEquivalentTo(srcCol);
        }

        [Fact]
        public void Should_update_item()
        {
            var srcCol = new List<SimpleObj>
            {
                new SimpleObj { Id = 1, StringValue = "string1" }
            };
            var destCol = new List<AnotherSimpleObj>
            {
                new AnotherSimpleObj { Id = 1, StringValue = "string2" }
            };


            destCol.AsCollectionUpdater(srcCol)
                .PairUsing((src, dest) => src.Id == dest.Id)
                .SetMapFunc((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.StringValue = src.StringValue;
                })
                .Update();


            destCol.Should().HaveCount(1);
            destCol.First().Should().BeEquivalentTo(new AnotherSimpleObj { Id = 1, StringValue = "string1" });
        }

        [Fact]
        public void Execute_custom_action_on_added_element()
        {
            var srcCol = new List<SimpleObj>
            {
                new SimpleObj { Id = 1, StringValue = "string1" }
            };
            var destCol = new List<AnotherSimpleObj> { };


            destCol.AsCollectionUpdater(srcCol)
                .PairUsing((src, dest) => src.Id == dest.Id)
                .SetMapFunc((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.StringValue = src.StringValue;
                })
                .DoOnAddedItem((src, dest) => dest.StringValue = "addedElement")
                .Update();


            destCol.Should().HaveCount(1);
            destCol.First().Should().BeEquivalentTo(new AnotherSimpleObj { Id = 1, StringValue = "addedElement" });
        }

        [Fact]
        public void Execute_custom_action_on_deleted_element()
        {
            var srcCol = new List<SimpleObj>
            {
                
            };
            var destCol = new List<AnotherSimpleObj>
            {
                new AnotherSimpleObj { Id = 1, StringValue = "string1" }
            };


            destCol.AsCollectionUpdater(srcCol)
                .PairUsing((src, dest) => src.Id == dest.Id)
                .SetMapFunc((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.StringValue = src.StringValue;
                })
                .DoOnRemovedItem(dest => dest.StringValue = "deletedElement")
                .Update();


            destCol.Should().HaveCount(1);
            destCol.First().Should().BeEquivalentTo(new AnotherSimpleObj { Id = 1, StringValue = "deletedElement" });
        }

        [Fact]
        public void Throws_exception_when_no_PairUsing_set()
        {
            Action act = () => new List<SimpleObj> {  new SimpleObj() }
                .AsCollectionUpdater(new List<SimpleObj> { new SimpleObj() })
                .SetMapFunc((src, dest) => { })
                .Update();

            act.Should().Throw<NotImplementedException>();
        }

        [Fact]
        public void Throws_exception_when_no_MapFunc_set()
        {
            Action act = () => new List<SimpleObj> { new SimpleObj() }
                .AsCollectionUpdater(new List<SimpleObj> { new SimpleObj() })
                .PairUsing((src, dest) => src == dest)
                .Update();

            act.Should().Throw<NotImplementedException>();
        }

        [Fact]
        public void Should_add_all_elements_from_src_if_dest_is_empty()
        {
            var srcCol = new List<SimpleObj>
            {
                new SimpleObj { Id = 0, StringValue = "string1" },
                new SimpleObj { Id = 0, StringValue = "string2" }
            };
            var destCol = new List<SimpleObj>();


            destCol.AsCollectionUpdater(srcCol)
                .PairUsing((src, dest) => src.Id == dest.Id)
                .SetMapFunc((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.StringValue = src.StringValue;
                })
                .Update();


            destCol.Should().HaveCount(2);
            destCol.Should().BeEquivalentTo(new List<SimpleObj>
            {
                new SimpleObj { Id = 0, StringValue = "string1" },
                new SimpleObj { Id = 0, StringValue = "string2" }
            });
        }
    }
}
