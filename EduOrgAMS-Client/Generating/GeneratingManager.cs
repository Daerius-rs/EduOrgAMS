﻿using System;
using EduOrgAMS.Client.Cryptography;
using EduOrgAMS.Client.Utils;
using RIS.Randomizing;
using RIS.Text.Generating;

namespace EduOrgAMS.Client.Generating
{
    public static class GeneratingManager
    {
        private static readonly string[] Smiles =
        {
            "(ﾟдﾟ；)",
            "(ó﹏ò｡)",
            "(´ω｀*)",
            "(┛ಠДಠ)┛彡┻━┻",
            "(* _ω_)…",
            "(ﾉ･д･)ﾉ",
            "(⊃｡•́‿•̀｡)⊃",
            "ლ(๏‿๏ ◝ლ)",
            "ლ(*꒪ヮ꒪*)ლ",
            "(ﾉ･ｪ･)ﾉ",
            "(＾▽＾)",
            "(•‿•)",
            "(☉_☉)",
            "(,,◕ ⋏ ◕,,)",
            "(๑❛ꇳ❛๑)",
            "(-, – )…zzzZZZ",
            "┬─┬ノ( º _ ºノ)",
            "(⌒‿⌒)",
            "\\ (•◡•) /",
            "⚆ _ ⚆",
            "(づ￣ ³￣)づ",
            "ಠ‿↼"
        };

        private static string _previousSmile;

        public static readonly IUnbiasedRandom RandomGenerator;
        public static readonly IUnbiasedRandom CachedRandomGenerator;
        public static readonly StringGenerator RandomStringGenerator;

        static GeneratingManager()
        {
            _previousSmile = string.Empty;

            RandomGenerator = new SecureRandom();
            CachedRandomGenerator = new CachedSecureRandom(
                1 * 1024 * 1024, 1, false);
            RandomStringGenerator = new StringGenerator(
                new SecureRandom());
        }

        public static string GetRandomSmile()
        {
            var biasZone =
                int.MaxValue - (int.MaxValue % Smiles.Length) - 1;
            int smileIndex =
                (int)CachedRandomGenerator
                    .GetUInt32((uint)biasZone) % Smiles.Length;

            if (Smiles[smileIndex] != _previousSmile)
            {
                _previousSmile = Smiles[smileIndex];
                return Smiles[smileIndex];
            }

            if (smileIndex == 0)
            {
                ++smileIndex;
            }
            else if (smileIndex == Smiles.Length - 1)
            {
                --smileIndex;
            }
            else
            {
                if (Rand.Current.NextBoolean(0.5))
                    ++smileIndex;
                else
                    --smileIndex;
            }

            _previousSmile = Smiles[smileIndex];
            return Smiles[smileIndex];
        }

        public static string GenerateToken(string login, ulong tokenExpirationDate)
        {
            var rawTokenData =
                $"{login}" +
                $"-{TimeUtils.ToUnixTimeStamp(DateTime.UtcNow)}" +
                $"-{tokenExpirationDate}" +
                $"-[{RandomStringGenerator.GenerateString(20, false)}]";
            var token = HashManager.Service
                .GetHash(rawTokenData);

            return token;
        }
    }
}
