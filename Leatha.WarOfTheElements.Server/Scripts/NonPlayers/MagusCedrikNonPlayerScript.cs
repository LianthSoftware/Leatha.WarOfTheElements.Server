using Leatha.WarOfTheElements.Common.Communication.Transfer;
using Leatha.WarOfTheElements.Server.DataAccess.Entities;
using Leatha.WarOfTheElements.Server.DataAccess.Entities.Templates;
using Leatha.WarOfTheElements.Server.Objects.Attributes;
using Leatha.WarOfTheElements.Server.Objects.Characters;
using Leatha.WarOfTheElements.Server.Objects.Characters.Movement;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace Leatha.WarOfTheElements.Server.Scripts.NonPlayers
{
    [ScriptName("magus_cedrik", ScriptType.NonPlayer)]
    public sealed class MagusCedrikNonPlayerScript : NonPlayerScriptBase
    {
        public MagusCedrikNonPlayerScript(NonPlayerState state, NonPlayerTemplate template) : base(state, template)
        {
        }

        private List<EventDataInfo> _texts = new List<EventDataInfo>
        {
            new EventDataInfo { Id = 1, Phase = 1, DelayMs = 6500, Text = "Welcome! I am always glad to see a new adept to master the elements.", MessageType = ChatMessageType.Yell},
            new EventDataInfo { Id = 2, Phase = 1, DelayMs = 6500, Text = "You must be wondering what are you doing at this strange place." },
            new EventDataInfo { Id = 3, Phase = 1, DelayMs = 6500, Text = "Well ... you are about to uncover the secrets of the magical elements." },
            new EventDataInfo { Id = 4, Phase = 1, DelayMs = 6500, Text = "Let's take a tour around this place, I will explain it while we have a nice little chat." },

            new EventDataInfo { Id = 5, Phase = 2, DelayMs = 6500, Text = "Ah, this one is a hot take! Hahaha ... ehm, okay." },
            new EventDataInfo { Id = 6, Phase = 2, DelayMs = 6500, Text = "Yes, if you choose the path of fire caller, you can strike your enemies with it pretty nicely." },
            new EventDataInfo { Id = 7, Phase = 2, DelayMs = 6500, Text = "Fire callers are the fierce killers from a reasonable distance." },

            new EventDataInfo { Id = 8, Phase = 3, DelayMs = 6500, Text = "Or, you can take upon the power of strong winds." },
            new EventDataInfo { Id = 9, Phase = 3, DelayMs = 6500, Text = "Whether you slice your enemies or shroud your friends with various mists, it's light choice to make." },
            new EventDataInfo { Id = 10, Phase = 3, DelayMs = 6500, Text = "Okay, I will stop joking." },
            new EventDataInfo { Id = 11, Phase = 3, DelayMs = 6500, Text = "... You have no sense of humour ...", MessageType = ChatMessageType.Mutter },

            new EventDataInfo { Id = 12, Phase = 4, DelayMs = 6500, Text = "Maybe you are rather cuddly person, wanting to blow your enemies into the nothingness from close range." },
            new EventDataInfo { Id = 13, Phase = 4, DelayMs = 6500, Text = "Just imagine, cloud shields? Lightning fists? You WANT that." },

            new EventDataInfo { Id = 14, Phase = 5, DelayMs = 6500, Text = "If I might guess, you will like this one. Healing others with nature power or shield them with mighty earth is always fun, right? Right?!" },
            new EventDataInfo { Id = 15, Phase = 5, DelayMs = 6500, Text = "As you skill up, your earth barriers or nature connection will *grow* with you" },
            new EventDataInfo { Id = 16, Phase = 5, DelayMs = 6500, Text = "Hehe ...", MessageType = ChatMessageType.Mutter },

            new EventDataInfo { Id = 17, Phase = 6, DelayMs = 6500, Text = "And I think you know the last element. The sound of flowing water, nothing is more relaxing!" },
            new EventDataInfo { Id = 18, Phase = 6, DelayMs = 6500, Text = "With this one, you will never be thirsty again! And what more, soaking your enemies in various liquids is useful anyway." },

            new EventDataInfo { Id = 19, Phase = 7, DelayMs = 6500, Text = "So, I introduced you to the elements. Now, choose wisely your primary element." },
            new EventDataInfo { Id = 20, Phase = 7, DelayMs = 6500, Text = "Eh? Oh, yeah..." },
            new EventDataInfo { Id = 21, Phase = 7, DelayMs = 4000, Text = "Just move to the one you want and interact with its power core." },
            new EventDataInfo { Id = 22, Phase = 7, DelayMs = 6500, Text = "Good luck!", MessageType = ChatMessageType.Whisper},
        };

        private int _eventPhase;
        //private int _textIndex = 0;
        private int _textId = 0;

        private double? _textWaitDelay = 0.0f;
        private double _currentDelay = 0.0f;

        public override void OnWaypointReached(int waypointIndex)
        {
            base.OnWaypointReached(waypointIndex);

            Debug.WriteLine($"Magus Cedrink - reached waypoing ({ waypointIndex }).");

            ++_eventPhase;
            AdvanceText();
        }

        private bool HasNextText()
        {
            var info = _texts.SingleOrDefault(i => i.Phase == _eventPhase && i.Id == _textId);
            return info != null;
        }

        private EventDataInfo? GetNextText()
        {
            var text = _texts.SingleOrDefault(i => i.Phase == _eventPhase && i.Id == _textId);

            Debug.WriteLine($"Getting Text with Phase = { _eventPhase } and { _textId } => { text?.Text }");

            return text;
        }

        private void AdvanceText()
        {
            if (_eventPhase <= 0)
                return;

            if (_eventPhase > _texts.Max(i => i.Phase))
            {
                Debug.WriteLine($"Event ended.");
                EndEvent();
                return;
            }

            //if (_textIndex >= _texts.Count)
            //{
            //    EndEvent();
            //    return;
            //}

            var info = GetNextText();
            if (info == null)
                return;

            Debug.WriteLine($"Advance text to \"{info.Text}\"");

            Talk(info.Text, info.MessageType, info.DelayMs / 1000.0f);
            _textId++;
            _currentDelay = info.DelayMs / 1000.0f;

            /*
             *- PositionX = 5f, PositionY = 1f, PositionZ = 5f
               - PositionX = 17f, PositionY = 1f, PositionZ = 22f
               - PositionX = -14.5f, PositionY = 1f, PositionZ = 25f
               - PositionX = -22.5f, PositionY = 1f, PositionZ = 0f
               - PositionX = -18f, PositionY = 1f, PositionZ = -22.5f
               - PositionX = 15f, PositionY = 1f, PositionZ = -24.75f
               - PositionX = 5f, PositionY = 1f, PositionZ = 5f
               
             *
             */

            // #TODO: Hacky, hacky ...
            if (_textId == 4)
            {
                State.MotionMaster.MoveWaypoints([
                        new WaypointData
                        {
                            PositionX = 17f, PositionY = 1f, PositionZ = 22f, DelayMin = 15000, DelayMax = 15000
                        },
                        new WaypointData
                        {
                            PositionX = -14.5f, PositionY = 1f, PositionZ = 25f, DelayMin = 13000,
                            DelayMax = 13000
                        },
                        new WaypointData
                        {
                            PositionX = -22.5f, PositionY = 1f, PositionZ = 0f, DelayMin = 20000,
                            DelayMax = 20000
                        },
                        new WaypointData
                        {
                            PositionX = -18f, PositionY = 1f, PositionZ = -22.5f, DelayMin = 12000,
                            DelayMax = 12000
                        },
                        new WaypointData
                        {
                            PositionX = 15f, PositionY = 1f, PositionZ = -24.75f, DelayMin = 12000,
                            DelayMax = 12000
                        },
                        new WaypointData
                        {
                            PositionX = 5f, PositionY = 1f, PositionZ = 5f, DelayMin = 12000, DelayMax = 12000
                        },
                    ],
                    false);
            }

            //Talk(_texts[_textIndex].Text, _texts[_textIndex].DelayMs / 1000.0f);
            //_currentDelay = _texts[_textIndex].DelayMs / 1000.0f;
            //_textIndex++;
        }

        public override void OnPlayerMovedToRadius(PlayerState playerState)
        {
            if (_eventPhase == 0)
                StartEvent();
        }

        public override void OnUpdate(double delta)
        {
            if (_eventPhase <= 0 || !_textWaitDelay.HasValue)
                return;

            if (!HasNextText())
                return;

            if (_textWaitDelay >= _currentDelay)
            {
                _textWaitDelay = 0.0f;
                AdvanceText();
            }
            else
                _textWaitDelay += delta;
        }

        private void StartEvent()
        {
            _eventPhase = 1;
            _textId = 1;

            AdvanceText();
        }

        private void EndEvent()
        {
            _textId = -1;
            _eventPhase = -1;
            _currentDelay = 0.0;
            _textWaitDelay = 0.0f;
        }

        //private void AdvanceTextA()
        //{
        //    if (_eventPhase <= 0)
        //        return;

        //    if (_textIndex >= _texts.Count)
        //    {
        //        EndEvent();
        //        return;
        //    }

        //    Debug.WriteLine($"Advance text to \"{ _texts[_textIndex].Text }\"");

        //    Talk(_texts[_textIndex].Text, _texts[_textIndex].DelayMs / 1000.0f);
        //    _currentDelay = _texts[_textIndex].DelayMs / 1000.0f;
        //    _textIndex++;

        //    // #TODO: Hacky, hacky ...
        //    if (_textIndex == 4)
        //    {
        //        State.MotionMaster.MoveWaypoints([
        //                new WaypointData { PositionX = 17.343f, PositionY = 1.4f, PositionZ = 20f, DelayMin = 6000, DelayMax = 6500 },
        //                new WaypointData { PositionX = -18.498f, PositionY = 1.4f, PositionZ = 20.0f, DelayMin = 4000, DelayMax = 4500 },
        //                new WaypointData { PositionX = -22.8f, PositionY = 1.4f, PositionZ = -0.223f, DelayMin = 4000, DelayMax = 4500 },
        //                new WaypointData { PositionX = -17.564f, PositionY = 1.4f, PositionZ = -21.72f, DelayMin = 5000, DelayMax = 5500 },
        //                new WaypointData { PositionX = 17.358f, PositionY = 1.4f, PositionZ = -21.72f, DelayMin = 4000, DelayMax = 4500 },
        //                new WaypointData { PositionX = 0.0f, PositionY = 1.4f, PositionZ = 0.0f, DelayMin = 0, DelayMax = 0 },
        //            ],
        //            false);
        //    }
        //}
    }

    public sealed class EventDataInfo
    {
        public string Text { get; set; } = null!;

        public int DelayMs { get; set; }

        public int Phase { get; set; }

        public int Id { get; set; }

        public ChatMessageType MessageType { get; set; } = ChatMessageType.Say;
    }
}
