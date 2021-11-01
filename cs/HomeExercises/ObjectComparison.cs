﻿using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible",
				54,
				170,
				70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Перепишите код на использование Fluent Assertions.
			Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
			Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
			Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
			Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

			Assert.AreEqual(expectedTsar.Parent!.Name, actualTsar.Parent!.Name);
			Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
			Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
			Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);

			//Переписал в RefactoredCheckCurrentTsar()
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible",
				54,
				170,
				70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			
			/* Недостатки
			 * 1. Изменение класса Person влечет за собой изменение метода AreEqual
			 * 2. Не очень читаемый код в AreEqual (я не сразу заметил, что там Id не сравниваются)
			 *
			 */
			
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

		private bool AreEqual(Person? actual, Person? expected)
		{
			if (actual == expected) return true;
			if (actual == null || expected == null) return false;
			return
				actual.Name == expected.Name &&
				actual.Age == expected.Age &&
				actual.Height == expected.Height &&
				actual.Weight == expected.Weight &&
				AreEqual(actual.Parent, expected.Parent);
		}

		[Test]
		[Category("Refactored")]
		public void RefactoredCheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible",
				54,
				170,
				70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
			//Регулярка находит строки формата "Id", "Parent.Id", "Parent.Parent.Id" и тд
			var idFieldRegex = new Regex("^((Parent\\.)*Id)$");

			actualTsar
				.Should()
				.BeEquivalentTo(
					expectedTsar,
					config => config.Excluding(ctx => idFieldRegex.IsMatch(ctx.SelectedMemberPath)));
		}
	}

	public class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
			return new Person(
				"Ivan IV The Terrible",
				54,
				170,
				70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person? Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person? parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}