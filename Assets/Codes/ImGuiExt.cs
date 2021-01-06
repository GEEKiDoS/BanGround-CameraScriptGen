using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImGuiNET
{
    public partial class ImGuiExt
    {
        [DllImport("cimgui", EntryPoint = "igInputScalar", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool InputScalarFloat(string label, ImGuiDataType data_type, ref float p_data, ref float p_step, ref float p_step_fast, string format, ImGuiInputTextFlags flags);

        public static void TextCenter(string text)
        {
            float font_size = ImGui.GetFontSize() * text.Length / 2;
            ImGui.SameLine(
                ImGui.GetWindowSize().x / 2 -
                font_size + (font_size / 2)
            );

            ImGui.Text(text);
        }

        public static bool ButtonCenter(string text)
        {
            ImGui.NewLine();
            float font_size = ImGui.GetFontSize() * text.Length / 2;
            ImGui.SameLine(
                ImGui.GetWindowSize().x / 2 -
                font_size + (font_size / 2)
            );

            return ImGui.Button(text);
        }
    }

}
