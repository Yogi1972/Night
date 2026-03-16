using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Rpg_Dungeon
{
    internal class Dialogue
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public List<DialogueNode>? Nodes { get; set; }
    }

    internal class DialogueNode
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public List<DialogueChoice>? Choices { get; set; }
        public bool IsEnd { get; set; }
    }

    internal class DialogueChoice
    {
        public string? Text { get; set; }
        public string? Next { get; set; }
        public string? Action { get; set; }
    }

    internal static class DialogueManager
    {
        // Start example dialogue and optionally operate on a Journal and NPCManager so dialogue actions
        // can add/modify quests. Both parameters may be null for read-only conversations.
        public static void StartExampleDialogue(Journal? journal = null, NPCManager? npcManager = null)
        {
            var path = Path.Combine("Data", "Dialogues", "example.json");
            var dlg = LoadDialogue(path);
            if (dlg == null || dlg.Nodes == null || dlg.Nodes.Count == 0)
            {
                Console.WriteLine("❌ No dialogue available.");
                return;
            }

            RunDialogue(dlg, journal, npcManager);
        }

        public static Dialogue? LoadDialogue(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                var json = File.ReadAllText(filePath);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Dialogue>(json, opts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to load dialogue: {ex.Message}");
                return null;
            }
        }

        private static void RunDialogue(Dialogue dialogue, Journal? journal, NPCManager? npcManager)
        {
            var nodes = dialogue.Nodes ?? new List<DialogueNode>();
            var nodeMap = new Dictionary<string, DialogueNode>();
            foreach (var n in nodes)
            {
                if (n?.Id != null) nodeMap[n.Id] = n;
            }

            // Start at first node in list if no explicit start
            var current = nodes.Count > 0 ? nodes[0].Id : null;
            while (current != null && nodeMap.TryGetValue(current, out var node))
            {
                Console.WriteLine($"\n{node.Text}");

                if (node.Choices == null || node.Choices.Count == 0 || node.IsEnd)
                {
                    Console.WriteLine("\n(End of conversation)");
                    break;
                }

                for (int i = 0; i < node.Choices.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {node.Choices[i].Text}");
                }

                Console.Write("Your choice: ");
                var choice = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(choice, out int idx) && idx > 0 && idx <= node.Choices.Count)
                {
                    var sel = node.Choices[idx - 1];
                    if (!string.IsNullOrEmpty(sel.Action))
                    {
                        // Handle simple action hooks: AcceptQuest:<name> and OfferQuest:<name>
                        Console.WriteLine($"[Action triggered: {sel.Action}]");
                        try
                        {
                            if (sel.Action.StartsWith("AcceptQuest:", StringComparison.OrdinalIgnoreCase) && journal != null)
                            {
                                var qname = sel.Action.Substring("AcceptQuest:".Length).Trim();
                                if (!string.IsNullOrEmpty(qname))
                                {
                                    Quest? quest = null;
                                    if (npcManager != null)
                                    {
                                        quest = npcManager.FindQuestByName(qname);
                                    }

                                    if (quest != null)
                                    {
                                        journal.AddActiveQuest(quest);
                                        // remove from NPC offerings so it's no longer available
                                        npcManager?.RemoveQuestByName(qname);
                                        Console.WriteLine($"✅ Quest '{qname}' accepted and added to your Journal.");
                                    }
                                    else
                                    {
                                        // fallback: create a minimal quest if not found
                                        var q = new Quest(qname, "Accepted via dialogue.", QuestType.Collect, QuestDifficulty.Easy, "Objective", 1, 0, 0);
                                        journal.AddActiveQuest(q);
                                        Console.WriteLine($"✅ Quest '{qname}' added to your Journal.");
                                    }
                                }
                            }
                            else if (sel.Action.StartsWith("OfferQuest:", StringComparison.OrdinalIgnoreCase) && journal != null)
                            {
                                var qname = sel.Action.Substring("OfferQuest:".Length).Trim();
                                if (!string.IsNullOrEmpty(qname))
                                {
                                    Console.WriteLine($"🔔 Quest offered: {qname} (check your Journal to accept)");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Action processing failed: {ex.Message}");
                        }
                    }

                    if (!string.IsNullOrEmpty(sel.Next))
                    {
                        current = sel.Next;
                        continue;
                    }
                    else
                    {
                        // No next node -> end
                        Console.WriteLine("\n(Conversation finished)");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
        }
    }
}
