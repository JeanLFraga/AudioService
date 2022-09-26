using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace JeanLF.AudioService.Tests
{
    public class AudioPoolTests
    {
        private AudioDatabase _database;
        private AudioPool _pool;
        private List<AudioPlayer> _players = new List<AudioPlayer>();

        [OneTimeSetUp]
        public void Setup()
        {
            AudioServiceSettings settings = Resources.Load<AudioServiceSettings>(TestUtility.AudioSettingsFile);
            _database = settings.Database;
        }

        [TearDown]
        public void CleanUp()
        {
            _pool.Dispose();
        }

        [Test]
        public void PlayerPoolReturnsValidObject()
        {
            PoolSettings settings = new PoolSettings(1, 0, 0, 0);
            _pool = new AudioPool(_database, settings);
            AudioPlayer player = _pool.GetPlayer();

            Assert.NotNull(player);
            Assert.NotNull(player.GetComponent<AudioSource>());

            _pool.ReleasePlayer(player);
        }

        [Test]
        public void FilterPoolReturnsValidObject()
        {
            EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);
            PoolSettings settings = new PoolSettings(0, 1, 0, 0);
            _pool = new AudioPool(_database, settings);

            AudioPlayer player = _pool.GetFilteredPlayer(entryId);
            AudioEntry entry = _database.AudioEntries.First(x => x.ConvertedId == entryId);

            IEnumerable<Type> filters = entry.Filters.Select(property => property.FilterType);
            IEnumerable<Type> components = player.GetComponents<Component>().Where(component => filters.Contains(component.GetType())).Select(component => component.GetType());

            Assert.NotNull(player);
            Assert.NotNull(player.GetComponent<AudioSource>());
            Assert.AreEqual(components, filters);

            _pool.ReleaseFilteredPlayer(entryId, player);
        }

        [Test]
        public void ThrownWhenPoolIsEmptyAndNotExpandable()
        {
            PoolSettings settings = new PoolSettings(0, 0, 0, 0);
            _pool = new AudioPool(_database, settings);
            Assert.Throws<InvalidOperationException>(() => _pool.GetPlayer());
        }

        [TestCase(2, 3)]
        [TestCase(5, 3)]
        [TestCase(1, 1)]
        public void PlayerPoolExpandWhenNeeded(int poolCount, int expandStep)
        {
            PoolSettings settings = new PoolSettings(poolCount, 0, expandStep, 0);
            _pool = new AudioPool(_database, settings);

            _players.Clear();
            for (int i = 0; i < poolCount; i++)
            {
                _players.Add(_pool.GetPlayer());
            }
            _players.Add(_pool.GetPlayer());

            int count = Mathf.Min(poolCount, expandStep);
            Assert.AreEqual(count - 1, _pool.GetPlayerInstanceCount());

            foreach (AudioPlayer audioPlayer in _players)
            {
                _pool.ReleasePlayer(audioPlayer);
            }
        }

        [TestCase(2, 3)]
        [TestCase(5, 3)]
        [TestCase(1, 1)]
        public void FilterPoolExpandWhenNeeded(int poolCount, int expandStep)
        {
            EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);

            PoolSettings settings = new PoolSettings(0, poolCount, expandStep, 0);
            _pool = new AudioPool(_database, settings);

            _players.Clear();
            for (int i = 0; i < poolCount; i++)
            {
                _players.Add(_pool.GetFilteredPlayer(entryId));
            }
            _players.Add(_pool.GetFilteredPlayer(entryId));

            int count = Mathf.Min(poolCount, expandStep);
            Assert.AreEqual(count - 1, _pool.GetFilteredPlayerInstanceCount(entryId));

            foreach (AudioPlayer audioPlayer in _players)
            {
                _pool.ReleaseFilteredPlayer(entryId ,audioPlayer);
            }
        }

        [Test]
        public void PlayerPoolShrinkWhenNeeded()
        {
            PoolSettings settings = new PoolSettings(1, 0, 1, 3);
            _pool = new AudioPool(_database, settings);

            _players.Clear();
            for (int i = 0; i < settings.ShrinkCount + 1; i++)
            {
                _players.Add(_pool.GetPlayer());
            }

            for (int i = 0; i < _players.Count; i++)
            {
                _pool.ReleasePlayer(_players[i]);
            }

            Assert.AreEqual(settings.PlayerPoolCount, _pool.GetPlayerInstanceCount());
        }

        [Test]
        public void FilterPoolShrinkWhenNeeded()
        {
            EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);

            PoolSettings settings = new PoolSettings(0, 1, 1, 3);
            _pool = new AudioPool(_database, settings);

            _players.Clear();
            for (int i = 0; i < settings.ShrinkCount + 1; i++)
            {
                _players.Add(_pool.GetFilteredPlayer(entryId));
            }

            for (int i = 0; i < _players.Count; i++)
            {
                _pool.ReleaseFilteredPlayer(entryId, _players[i]);
            }

            Assert.AreEqual(settings.FilterPlayerPoolCount, _pool.GetFilteredPlayerInstanceCount(entryId));
        }
    }
}
