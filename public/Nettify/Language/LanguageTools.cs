//
// Nettify  Copyright (C) 2023-2024  Aptivi
//
// This file is part of Nettify
//
// Nettify is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Nettify is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using LocaleStation.Tools;
using Nettify.Localized;

namespace Nettify.Language
{
    internal static class LanguageTools
    {
        private const string localType = "Nettify";

        internal static string GetLocalized(string id)
        {
            if (!LanguageCommon.IsCustomActionDefined(localType))
                LanguageCommon.AddCustomAction(localType, new(() => LocalStrings.Languages, () => LocalStrings.Localizations, LocalStrings.Translate, LocalStrings.CheckCulture, LocalStrings.ListLanguagesCulture, LocalStrings.Exists));
            return LanguageCommon.Translate(id);
        }
    }
}
