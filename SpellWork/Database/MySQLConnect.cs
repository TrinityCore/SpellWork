using MySql.Data.MySqlClient;
using SpellWork.DBC.Structures;
using SpellWork.Properties;
using SpellWork.Spell;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpellWork.Database
{
    public static class MySqlConnection
    {
        public static bool Connected { get; private set; }
        public static List<string> Dropped = new List<string>();
        public static List<SpellProcEntry> SpellProcEvent = new List<SpellProcEntry>();

        private static string ConnectionString
        {
            get
            {
                if (Settings.Default.Host == ".")
                    return
                        $"Server=localhost;Pipe={Settings.Default.PortOrPipe};UserID={Settings.Default.User};Password={Settings.Default.Pass};Database={Settings.Default.WorldDbName};CharacterSet=utf8mb4;ConnectionTimeout=5;ConnectionProtocol=Pipe;";

                return
                    $"Server={Settings.Default.Host};Port={Settings.Default.PortOrPipe};UserID={Settings.Default.User};Password={Settings.Default.Pass};Database={Settings.Default.WorldDbName};CharacterSet=utf8mb4;ConnectionTimeout=5;";
            }
        }

        private static string GetSpellName(uint id)
        {
            if (DBC.DBC.SpellInfoStore.ContainsKey((int)id))
                return DBC.DBC.SpellInfoStore[(int)id].NameAndSubname;

            Dropped.Add($"DELETE FROM `spell_proc` WHERE `SpellId`={id};\r\n");
            return string.Empty;
        }

        public static void SelectProc(string query)
        {
            if (!Connected)
                return;

            Dropped.Clear();
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var command = new MySqlCommand(query, conn))
                {
                    SpellProcEvent.Clear();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var spellId = reader.GetInt32(0);
                            SpellProcEvent.Add(new SpellProcEntry
                            {
                                SpellId              = spellId,
                                SpellName            = GetSpellName((uint)Math.Abs(spellId)),
                                SchoolMask           = (SpellSchoolMask)reader.GetByte(1),
                                SpellFamilyName      = (SpellFamilyNames)reader.GetUInt16(2),
                                SpellFamilyMask      = new[]
                                {
                                    reader.GetUInt32(3),
                                    reader.GetUInt32(4),
                                    reader.GetUInt32(5),
                                    reader.GetUInt32(6)
                                },
                                ProcFlags            = (ProcFlags)reader.GetUInt32(7),
                                ProcFlags2           = (ProcFlags2)reader.GetUInt32(8),
                                SpellTypeMask        = (ProcFlagsSpellType)reader.GetUInt32(9),
                                SpellPhaseMask       = (ProcFlagsSpellPhase)reader.GetUInt32(10),
                                HitMask              = (ProcFlagsHit)reader.GetUInt32(11),
                                AttributesMask       = (ProcAttributes)reader.GetUInt32(12),
                                DisableEffectsMask   = reader.GetUInt32(13),
                                ProcsPerMinute       = reader.GetFloat(14),
                                Chance               = reader.GetFloat(15),
                                Cooldown             = reader.GetUInt32(16),
                                Charges              = reader.GetByte(17)
                            });
                        }
                    }
                }
            }
        }

        public static void LoadServersideSpells()
        {
            if (!Connected)
                return;

            var spellsQuery = "SELECT Id, DifficultyID, CategoryId, Dispel, Mechanic, Attributes, AttributesEx, AttributesEx2, AttributesEx3, " +
                "AttributesEx4, AttributesEx5, AttributesEx6, AttributesEx7, AttributesEx8, AttributesEx9, AttributesEx10, AttributesEx11, AttributesEx12, AttributesEx13, " +
                "AttributesEx14, AttributesEx15, AttributesEx16, Stances, StancesNot, Targets, TargetCreatureType, RequiresSpellFocus, FacingCasterFlags, " +
                "CasterAuraState, TargetAuraState, ExcludeCasterAuraState, ExcludeTargetAuraState, CasterAuraSpell, TargetAuraSpell, ExcludeCasterAuraSpell, ExcludeTargetAuraSpell, " +
                "CasterAuraType, TargetAuraType, ExcludeCasterAuraType, ExcludeTargetAuraType, CastingTimeIndex, " +
                "RecoveryTime, CategoryRecoveryTime, StartRecoveryCategory, StartRecoveryTime, InterruptFlags, AuraInterruptFlags1, AuraInterruptFlags2, " +
                "ChannelInterruptFlags1, ChannelInterruptFlags2, ProcFlags, ProcFlags2, ProcChance, ProcCharges, ProcCooldown, ProcBasePPM, MaxLevel, BaseLevel, SpellLevel, " +
                "DurationIndex, RangeIndex, Speed, LaunchDelay, StackAmount, EquippedItemClass, EquippedItemSubClassMask, EquippedItemInventoryTypeMask, ContentTuningId, " +
                "SpellName, ConeAngle, ConeWidth, MaxTargetLevel, MaxAffectedTargets, SpellFamilyName, SpellFamilyFlags1, SpellFamilyFlags2, SpellFamilyFlags3, SpellFamilyFlags4, " +
                "DmgClass, PreventionType, AreaGroupId, SchoolMask, ChargeCategoryId FROM serverside_spell";

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var command = new MySqlCommand(spellsQuery, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var difficulty = reader.GetUInt32(1);
                            if (difficulty != 0)
                                continue;

                            var spellId = reader.GetUInt32(0);
                            var spellInfo = new SpellInfo(reader.GetString(68) + " (SERVERSIDE)",
                                new SpellEntry()
                                {
                                    ID = spellId
                                });

                            spellInfo.AuraOptions = new SpellAuraOptionsEntry()
                            {
                                CumulativeAura = (ushort)reader.GetUInt32(63),
                                ProcCategoryRecovery = (int)reader.GetUInt32(54),
                                ProcChance = (byte)reader.GetUInt32(52),
                                ProcCharges = (int)reader.GetUInt32(53),
                                ProcTypeMask = new[] { (int)reader.GetUInt32(50), (int)reader.GetUInt32(51) }
                            };

                            spellInfo.AuraRestrictions = new SpellAuraRestrictionsEntry()
                            {
                                CasterAuraState = (byte)reader.GetUInt32(28),
                                TargetAuraState = (byte)reader.GetUInt32(29),
                                ExcludeCasterAuraState = (byte)reader.GetUInt32(30),
                                ExcludeTargetAuraState = (byte)reader.GetUInt32(31),
                                CasterAuraSpell = (int)reader.GetUInt32(32),
                                TargetAuraSpell = (int)reader.GetUInt32(33),
                                ExcludeCasterAuraSpell = (int)reader.GetUInt32(34),
                                ExcludeTargetAuraSpell = (int)reader.GetUInt32(35),
                                CasterAuraType = (short)reader.GetUInt32(36),
                                TargetAuraType = (short)reader.GetUInt32(37),
                                ExcludeCasterAuraType = (short)reader.GetUInt32(38),
                                ExcludeTargetAuraType = (short)reader.GetUInt32(39)
                            };

                            spellInfo.CastingRequirements = new SpellCastingRequirementsEntry()
                            {
                                FacingCasterFlags = (byte)reader.GetUInt32(27),
                                RequiredAreasID = (ushort)reader.GetInt32(80),
                                RequiresSpellFocus = (ushort)reader.GetUInt32(26)
                            };

                            spellInfo.Categories = new SpellCategoriesEntry()
                            {
                                Category = (short)reader.GetUInt32(2),
                                DefenseType = (sbyte)reader.GetUInt32(78),
                                DispelType = (sbyte)reader.GetUInt32(3),
                                Mechanic = (sbyte)reader.GetUInt32(4),
                                PreventionType = (sbyte)reader.GetUInt32(79),
                                StartRecoveryCategory = (short)reader.GetUInt32(43),
                                ChargeCategory = (short)reader.GetUInt32(82)
                            };

                            spellInfo.ClassOptions = new SpellClassOptionsEntry()
                            {
                                SpellClassSet = (byte)reader.GetUInt32(73),
                                SpellClassMask = new[]
                                {
                                    (int)reader.GetUInt32(74),
                                    (int)reader.GetUInt32(75),
                                    (int)reader.GetUInt32(76),
                                    (int)reader.GetUInt32(77)
                                }
                            };

                            spellInfo.Cooldowns = new SpellCooldownsEntry()
                            {
                                CategoryRecoveryTime = (int)reader.GetUInt32(42),
                                RecoveryTime = (int)reader.GetUInt32(41),
                                StartRecoveryTime = (int)reader.GetUInt32(44)
                            };

                            spellInfo.EquippedItems = new SpellEquippedItemsEntry()
                            {
                                EquippedItemClass = reader.GetInt32(64),
                                EquippedItemInvTypes = reader.GetInt32(66),
                                EquippedItemSubclass = reader.GetInt32(65)
                            };

                            spellInfo.Interrupts = new SpellInterruptsEntry()
                            {
                                InterruptFlags = (short)reader.GetUInt32(45),
                                AuraInterruptFlags = new[]
                                {
                                    (int)reader.GetUInt32(46),
                                    (int)reader.GetUInt32(47)
                                },
                                ChannelInterruptFlags = new[]
                                {
                                    (int)reader.GetUInt32(48),
                                    (int)reader.GetUInt32(49)
                                }
                            };

                            spellInfo.Levels = new SpellLevelsEntry()
                            {
                                MaxLevel = (short)reader.GetUInt32(56),
                                BaseLevel = (int)reader.GetUInt32(57),
                                SpellLevel = (int)reader.GetUInt32(58)
                            };

                            spellInfo.Misc = new SpellMiscEntry()
                            {
                                Attributes = new[]
                                {
                                    (int)reader.GetUInt32(5),
                                    (int)reader.GetUInt32(6),
                                    (int)reader.GetUInt32(7),
                                    (int)reader.GetUInt32(8),
                                    (int)reader.GetUInt32(9),
                                    (int)reader.GetUInt32(10),
                                    (int)reader.GetUInt32(11),
                                    (int)reader.GetUInt32(12),
                                    (int)reader.GetUInt32(13),
                                    (int)reader.GetUInt32(14),
                                    (int)reader.GetUInt32(15),
                                    (int)reader.GetUInt32(16),
                                    (int)reader.GetUInt32(17),
                                    (int)reader.GetUInt32(18),
                                    (int)reader.GetUInt32(19),
                                    (int)reader.GetUInt32(20),
                                    (int)reader.GetUInt32(21)
                                },
                                CastingTimeIndex = (ushort)reader.GetUInt32(40),
                                DurationIndex = (ushort)reader.GetUInt32(59),
                                RangeIndex = (ushort)reader.GetUInt32(60),
                                SchoolMask = (byte)reader.GetUInt32(81),
                                Speed = reader.GetFloat(61),
                                LaunchDelay = reader.GetFloat(62),
                                ContentTuningID = (int)reader.GetUInt32(67)
                            };

                            spellInfo.ProcsPerMinute = new SpellProcsPerMinuteEntry()
                            {
                                BaseProcRate = reader.GetFloat(55)
                            };

                            spellInfo.Shapeshift = new SpellShapeshiftEntry()
                            {
                                ShapeshiftMask = new[]
                                {
                                    (int)(reader.GetUInt64(22) & 0xFFFFFFFF),
                                    (int)(reader.GetUInt64(22) >> 32)
                                },
                                ShapeshiftExclude = new[]
                                {
                                    (int)(reader.GetUInt64(23) & 0xFFFFFFFF),
                                    (int)(reader.GetUInt64(23) >> 32)
                                },
                            };

                            spellInfo.TargetRestrictions = new SpellTargetRestrictionsEntry()
                            {
                                ConeDegrees = reader.GetFloat(69),
                                MaxTargets = (byte)reader.GetUInt32(72),
                                MaxTargetLevel = reader.GetUInt32(71),
                                TargetCreatureType = (short)reader.GetUInt32(25),
                                Targets = (int)reader.GetUInt32(24),
                                Width = reader.GetFloat(70)
                            };

                            if (DBC.DBC.SpellDuration.TryGetValue(spellInfo.Misc.DurationIndex, out var duration))
                                spellInfo.DurationEntry = duration;

                            if (DBC.DBC.SpellRange.TryGetValue(spellInfo.Misc.RangeIndex, out var range))
                                spellInfo.Range = range;

                            DBC.DBC.SpellInfoStore[(int)spellId] = spellInfo;
                        }
                    }
                }

                var effectsQuery =
                    "SELECT SpellID, EffectIndex, DifficultyID, Effect, EffectAura, EffectAmplitude, EffectAttributes, " +
                    "EffectAuraPeriod, EffectBonusCoefficient, EffectChainAmplitude, EffectChainTargets, EffectItemType, EffectMechanic, EffectPointsPerResource, " +
                    "EffectPosFacing, EffectRealPointsPerLevel, EffectTriggerSpell, BonusCoefficientFromAP, PvpMultiplier, Coefficient, Variance, " +
                    "ResourceCoefficient, GroupSizeBasePointsCoefficient, EffectBasePoints, EffectMiscValue1, EffectMiscValue2, EffectRadiusIndex1, " +
                    "EffectRadiusIndex2, EffectSpellClassMask1, EffectSpellClassMask2, EffectSpellClassMask3, EffectSpellClassMask4, ImplicitTarget1, " +
                    "ImplicitTarget2 FROM serverside_spell_effect";

                using (var command = new MySqlCommand(effectsQuery, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var difficulty = reader.GetUInt32(2);
                            if (difficulty != 0)
                                continue;

                            var spellId = reader.GetUInt32(0);
                            if (!DBC.DBC.SpellInfoStore.TryGetValue((int)spellId, out var spellInfo))
                                continue;

                            var effect = new SpellEffectEntry()
                            {
                                EffectIndex = reader.GetInt32(1),
                                Effect = reader.GetInt32(3),
                                EffectAura = reader.GetInt16(4),
                                EffectAmplitude = reader.GetFloat(5),
                                EffectAttributes = reader.GetInt32(6),
                                EffectAuraPeriod = reader.GetInt32(7),
                                EffectBonusCoefficient = reader.GetFloat(8),
                                EffectChainAmplitude = reader.GetFloat(9),
                                EffectChainTargets = reader.GetInt32(10),
                                EffectItemType = reader.GetInt32(11),
                                EffectMechanic = reader.GetInt32(12),
                                EffectPointsPerResource = reader.GetFloat(13),
                                EffectPosFacing = reader.GetFloat(14),
                                EffectRealPointsPerLevel = reader.GetFloat(15),
                                EffectTriggerSpell = reader.GetInt32(16),
                                BonusCoefficientFromAP = reader.GetFloat(17),
                                PvpMultiplier = reader.GetFloat(18),
                                Coefficient = reader.GetFloat(19),
                                Variance = reader.GetFloat(20),
                                ResourceCoefficient = reader.GetFloat(21),
                                GroupSizeBasePointsCoefficient = reader.GetFloat(22),
                                EffectBasePoints = reader.GetFloat(23),
                                EffectMiscValue = new[]
                                {
                                    reader.GetInt32(24),
                                    reader.GetInt32(25)
                                },
                                EffectRadiusIndex = new[]
                                {
                                    reader.GetUInt32(26),
                                    reader.GetUInt32(27),
                                },
                                EffectSpellClassMask = new[]
                                {
                                    reader.GetInt32(28),
                                    reader.GetInt32(29),
                                    reader.GetInt32(30),
                                    reader.GetInt32(31)
                                },
                                ImplicitTarget = new[]
                                {
                                    reader.GetInt16(32),
                                    reader.GetInt16(33)
                                },
                                SpellID = (int)spellId
                            };

                            spellInfo.SpellEffectInfoStore.Add(new SpellEffectInfo(effect));
                        }
                    }
                }
            }
        }

        public static void Insert(string query)
        {
            if (!Connected || Settings.Default.DbIsReadOnly)
                return;

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (var command = new MySqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void TestConnect()
        {
            if (!Settings.Default.UseDbConnect)
            {
                Connected = false;
                return;
            }

            try
            {
                using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    conn.Close();
                }
                Connected = true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Errno {ex.Number}{Environment.NewLine}{ex.Message}");
                Connected = false;
            }
            catch
            {
                Connected = false;
            }
        }
    }
}
