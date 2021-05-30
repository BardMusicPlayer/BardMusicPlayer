/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities
{
    internal class EnsembleMessageLookup
    {
        internal enum EnsembleFlag
        {
            None,
            Request,
            Start,
            Stop,
            Reject
        }

        internal static EnsembleFlag GetEnsembleFlag(string line)
        {
            return line switch
            {
                "Initiating ensemble mode ready check."                   => EnsembleFlag.Request,
                "Bereitschaftsanfrage für die Aufführung wird gestartet." => EnsembleFlag.Request,
                "Lancement de l'appel de préparation."                    => EnsembleFlag.Request,
                "合奏レディチェックを開始します。"                                        => EnsembleFlag.Request,
                // need kr
                // need cn

                "All party members are ready.The count-in will now commence." => EnsembleFlag.Start,
                "Alle Gruppenmitglieder bereit.Das Anzählen beginnt."         => EnsembleFlag.Start,
                "L'orchestre est prêt. Début du décompte."                    => EnsembleFlag.Start,
                "合奏レディチェックが完了しました。カウントを開始します。"                                => EnsembleFlag.Start,
                // need kr
                // need cn

                "Ensemble mode has ended."         => EnsembleFlag.Stop,
                "Der Ensemblemodus wurde beendet." => EnsembleFlag.Stop,
                "Le mode orchestral a pris fin."   => EnsembleFlag.Stop,
                "合奏モードが終了しました。"                    => EnsembleFlag.Stop,
                // need kr
                // need cn

                "Not all party members are ready.The ensemble mode ready check will be canceled." =>
                    EnsembleFlag.Reject,
                "Nicht alle Gruppenmitglieder bereit.Die Bereitschaftsanfrage wird beendet." => EnsembleFlag.Reject,
                "Un ou plusieurs membres de l'orchestre ne sont pas prêts."                  => EnsembleFlag.Reject,
                "メンバーの準備が完了していませんでした。合奏レディチェックを終了します。"                                       => EnsembleFlag.Reject,
                // need kr
                // need cn

                "Ensemble mode ready check canceled due to lack of eligible members." => EnsembleFlag.Reject,
                // need gr
                // need fr
                // need jp
                // need kr
                // need cn

                // Unused.
                "A member of the ensemble has stopped the metronome."     => EnsembleFlag.None,
                "Ein Mitglied des Ensembles hat das Metronom angehalten." => EnsembleFlag.None,
                "Le métronome a été arrêté par un membre de l'orchestre." => EnsembleFlag.None,
                "演奏メンバーからメトロノームが停止されました。"                                 => EnsembleFlag.None,
                // need kr
                // need cn

                // Unused
                "The ensemble mode metronome has stopped."                                   => EnsembleFlag.None,
                "Das Metronom für den Ensemblemodus wurde angehalten."                       => EnsembleFlag.None,
                "Votre métronome a été arrêté suite au lancement de l'appel de préparation." => EnsembleFlag.None,
                "合奏レディチェックでのメトロノームを停止しました。"                                                  => EnsembleFlag.None,
                // need kr
                // need cn

                _ => EnsembleFlag.None
            };
        }
    }
}