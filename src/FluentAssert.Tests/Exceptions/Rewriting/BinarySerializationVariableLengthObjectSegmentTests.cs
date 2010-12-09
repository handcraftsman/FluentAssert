﻿//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System.IO;

using FluentAssert.Exceptions.Rewriting;

using NUnit.Framework;

namespace FluentAssert.Tests.Exceptions.Rewriting
{
	public class BinarySerializationVariableLengthObjectSegmentTests
	{
		[TestFixture]
		public class When_asked_if_IsMatch
		{
			private int _initialPosition;
			private bool _result;
			private IBinarySerializationSegment _segment;
			private MemoryStream _stream;

			[SetUp]
			public void BeforeEachTest()
			{
				byte[] bytes = {
				               	0x06, 0x20
				               };

				_stream = new MemoryStream(bytes)
				{
					Position = 0
				};
				_segment = new BinarySerializationVariableLengthObjectSegment();
			}

			[Test]
			public void Given_a_MemoryStream_having_a_non_matching_type_id_at_Position()
			{
				Test.Verify(
					with_non_matching_type_id_at_Position,
					when_asked_if_IsMatch,
					should_return_false,
					should_not_change_Position
					);
			}

			[Test]
			public void Given_a_MemoryStream_having_byte_at_Position_equal_to_0x06()
			{
				Test.Verify(
					with_type_id_at_position_equal_to_0x06,
					when_asked_if_IsMatch,
					should_return_true,
					should_not_change_Position
					);
			}

			private void should_not_change_Position()
			{
				_stream.Position.ShouldBeEqualTo(_initialPosition);
			}

			private void should_return_false()
			{
				_result.ShouldBeFalse();
			}

			private void should_return_true()
			{
				_result.ShouldBeTrue();
			}

			private void when_asked_if_IsMatch()
			{
				_result = _segment.IsMatch(_stream);
			}

			private void with_non_matching_type_id_at_Position()
			{
				_stream.Position = _initialPosition = 1;
			}

			private void with_type_id_at_position_equal_to_0x06()
			{
				_stream.Position = _initialPosition = 0;
			}
		}

		[TestFixture]
		public class When_asked_to_Skip
		{
			private int _expectedPosition;
			private IBinarySerializationSegment _segment;
			private MemoryStream _stream;

			[SetUp]
			public void BeforeEachTest()
			{
				_segment = new BinarySerializationVariableLengthObjectSegment();
			}

			[Test]
			public void Given_a_stream_positioned_at_a_match_that_ends_with_segment_typeId_0x09()
			{
				var bytes = EmbeddedResource.Read("ShouldBeEqualAssertionException.bin");
				_stream = new MemoryStream(bytes)
				{
					Position = 0x1aa
				};
				_expectedPosition = 0x2cd;
				Test.Verify(
					when_asked_to_Skip,
					should_change_the_Position_to_the_first_byte_after_the_section
					);
			}

			[Test]
			public void Given_a_stream_positioned_at_a_match_that_ends_with_segment_typeId_0x0a()
			{
				var bytes = EmbeddedResource.Read("AssertionException.bin");
				_stream = new MemoryStream(bytes)
				{
					Position = 0x15f
				};
				_expectedPosition = 0x202;
				Test.Verify(
					when_asked_to_Skip,
					should_change_the_Position_to_the_first_byte_after_the_section
					);
			}

			private void should_change_the_Position_to_the_first_byte_after_the_section()
			{
				_stream.Position.ShouldBeEqualTo(_expectedPosition);
			}

			private void when_asked_to_Skip()
			{
				_segment.Skip(_stream);
			}
		}
	}
}