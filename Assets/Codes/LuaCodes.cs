using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;

public static class LuaCodes
{
    public static readonly string AnimatorCode = "function IIF(a,b,c)if a then return b else return c end end;local d=math.pi/2;local e={[0]=function(f)return IIF(f<1.0,0,1)end,[1]=function(f)return f end,[2]=function(f)return f*f end,[3]=function(f)return-(f*(f-2))end,[4]=function(f)if f<0.5 then return 2*f*f else return-2*f*f+4*f-1 end end,[5]=function(f)return f*f*f end,[6]=function(f)local g=f-1;return g*g*g+1 end,[7]=function(f)if f<0.5 then return 4*f*f*f else local g=2*f-2;return 0.5*g*g*g+1 end end,[8]=function(f)return f*f*f*f end,[9]=function(f)local g=f-1;return g*g*g*(1-f)+1 end,[10]=function(f)if f<0.5 then return 8*f*f*f*f else local g=f-1;return-8*g*g*g*g+1 end end,[11]=function(f)return f*f*f*f*f end,[12]=function(f)local g=f-1;return g*g*g*g*g+1 end,[13]=function(f)if f<0.5 then return 16*f*f*f*f*f else local g=2*f-2;return 0.5*g*g*g*g*g+1 end end,[14]=function(f)return math.sin((f-1)*d)+1 end,[15]=function(f)return math.sin(f*d)end,[16]=function(f)return 0.5*(1-math.cos(f*math.pi))end,[17]=function(f)return 1-math.sqrt(1-f*f)end,[18]=function(f)return math.sqrt((2-f)*f)end,[19]=function(f)if f<0.5 then return 0.5*(1-math.sqrt(1-4*f*f))else return 0.5*(math.sqrt(-(2*f-3)*(2*f-1))+1)end end,[20]=function(f)return IIF(f==0,f,2^(10*(f-1)))end,[21]=function(f)return IIF(f==1,f,1-2^(-10*f))end,[22]=function(f)if f==0 or f==1 then return f end;if f<0.5 then return 0.5*2^(20*f-10)else return-0.5*2^(-20*f+10)+1 end end,[23]=function(f)return math.sin(13*d*f)*2^(10*(f-1))end,[24]=function(f)return math.sin(-13*d*(f+1))*2^(-10*f)+1 end,[25]=function(f)if f<0.5 then return 0.5*math.sin(13*d*2*f)*2^(10*(2*f-1))else return 0.5*(math.sin(-13*d*(2*f-1+1))*2^(-10*(2*f-1))+2)end end,[26]=function(f)return f*f*f-f*math.sin(f*math.pi)end,[27]=function(f)local g=1-f;return 1-(g*g*g-g*math.sin(g*math.pi))end,[28]=function(f)if f<0.5 then local g=2*f;return 0.5*(g*g*g-g*math.sin(g*math.pi))else local g=1-(2*f-1)return 0.5*(1-(g*g*g-g*math.sin(g*math.pi)))+0.5 end end}e[30]=function(f)if f<4/11.0 then return 121*f*f/16.0 elseif f<8/11.0 then return 363/40.0*f*f-99/10.0*f+17/5.0 elseif f<9/10.0 then return 4356/361.0*f*f-35442/1805.0*f+16061/1805.0 else return 54/5.0*f*f-513/25.0*f+268/25.0 end end;e[29]=function(f)return 1-e[30](1-f)end;e[31]=function(f)if f<0.5 then return 0.5*e[29](f*2)else return 0.5*e[30](f*2-1)+0.5 end end;function Lerp(b,c,h)return b+(c-b)*h end;function UpdateObject(i,j)if i<0 then return end;local k=false;if#j.KeyFrames<j.LastKeyFrame or j.LastKeyFrame<1 then j.LastKeyFrame=1 end;if#j.KeyFrames==j.LastKeyFrame then return end;if j.KeyFrames[j.LastKeyFrame].Time>i then k=true end;local l=0;local m=0;local n=1;local o=-1;if not k then n=#j.KeyFrames;o=1 end;for p=j.LastKeyFrame,n,o do if j.KeyFrames[p].Time==i then l=p;m=p;break end;if j.KeyFrames[p].Time<i then l=p;if k then break end end;if j.KeyFrames[p].Time>i then m=p;if not k then break end end end;if m==0 or l==0 or l==m then local q=m;if m==0 then q=l end;local r=j.KeyFrames[q]j.LastKeyFrame=q;j.Object:SetPosition(r.X,r.Y,r.Z)j.Object:SetRotation(r.Pitch,r.Yaw,r.Roll)if not j.IsCamera then j.Object:SetColor(r.R,r.G,r.B,r.A)j.Object:SetScale(r.SX,r.SY,r.SZ)end else local s=j.KeyFrames[l]local t=j.KeyFrames[m]j.LastKeyFrame=l;local u=(i-s.Time)/(t.Time-s.Time)local v=e[t.Mode]j.Object:SetPosition(Lerp(s.X,t.X,v(u)),Lerp(s.Y,t.Y,v(u)),Lerp(s.Z,t.Z,v(u)))j.Object:SetRotation(Lerp(s.Pitch,t.Pitch,v(u)),Lerp(s.Yaw,t.Yaw,v(u)),Lerp(s.Roll,t.Roll,v(u)))if not j.IsCamera then j.Object:SetColor(Lerp(s.R,t.R,v(u)),Lerp(s.G,t.G,v(u)),Lerp(s.B,t.B,v(u)),Lerp(s.A,t.A,v(u)))j.Object:SetScale(Lerp(s.SX,t.SX,v(u)),Lerp(s.SY,t.SY,v(u)),Lerp(s.SZ,t.SZ,v(u)))end end end";

    public static void AppendLevel(this StringBuilder sb, int level)
    {
        for (int i = 0; i < level; i++)
            sb.Append("    ");
    }

    public static void KeyFrames(this StringBuilder stringBuilder, List<KeyFrame> keyFrames, bool isCamera, int level)
    {
        stringBuilder.AppendLine("{");
        ++level;

        for (int i = 0; i < keyFrames.Count; i++)
        {
            KeyFrame keyFrame = keyFrames[i];
            stringBuilder.AppendLevel(level);
            stringBuilder.Append(string.Format("[{0}] = {{ Time = {1}, Mode = {2}, X = {3}, Y = {4}, Z = {5}, Pitch = {6}, Yaw = {7}, Roll = {8}",
                i + 1,
                keyFrame.Time,
                keyFrame.InterpolationMode,
                keyFrame.Position.x,
                keyFrame.Position.y,
                keyFrame.Position.z,
                keyFrame.Rotation.x,
                keyFrame.Rotation.y,
                keyFrame.Rotation.z
            ));

            if (isCamera)
            {
                stringBuilder.AppendLine(" },");
            }
            else
            {
                stringBuilder.AppendLine(string.Format(", SX = {0}, SY = {1}, SZ = {2}, R = {3}, G = {4}, B = {5}, A = {6} }},",
                       keyFrame.Scale.x,
                       keyFrame.Scale.y,
                       keyFrame.Scale.z,
                       keyFrame.Color.x,
                       keyFrame.Color.y,
                       keyFrame.Color.z,
                       keyFrame.Color.w
                   ));
            }
        }

        --level;
        stringBuilder.AppendLevel(level);
        stringBuilder.AppendLine("},");
    }

    public static void ScriptObject(this StringBuilder sb, List<ScriptObject> objects)
    {
        sb.AppendLine("local textures = {");
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];

            if (obj.IsCamera)
                continue;

            sb.AppendLevel(1);
            sb.AppendLine($"[{i + 1}] = BanGround:LoadTexture(\"{Path.GetFileName(obj.TextureName)}\"),");
        }
        sb.AppendLine("};");

        sb.AppendLine();

        sb.AppendLine("local objects = {");

        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];

            sb.AppendLevel(1);
            sb.AppendLine($"[{i + 1}] = {{");

            sb.AppendLevel(2);
            sb.AppendLine($"IsCamera = {obj.IsCamera.ToString().ToLower()},");

            sb.AppendLevel(2);
            if (obj.IsCamera)
                sb.AppendLine("Object = BanGround:GetCamera(),");
            else
                sb.AppendLine($"Object = BanGround:CreateSprite(textures[{i + 1}]),");

            sb.AppendLevel(2);
            sb.AppendLine("LastKeyFrame = 0,");

            sb.AppendLevel(2);
            sb.Append("KeyFrames = ");
            sb.KeyFrames(obj.KeyFrames.ToList(), obj.IsCamera, 2);

            sb.AppendLevel(1);
            sb.AppendLine("},");
        }

        sb.AppendLine("};");
    }

    public static void OnUpdateStub(this StringBuilder sb)
    {
        sb.AppendLine("function OnUpdate(audioTime)");
        sb.AppendLevel(1);
        sb.AppendLine("local audioTimeS = audioTime / 1000;");
        sb.AppendLine();
        sb.AppendLevel(1);
        sb.AppendLine("for i = 1, #objects do");
        sb.AppendLevel(2);
        sb.AppendLine("UpdateObject(audioTimeS, objects[i]);");
        sb.AppendLevel(1);
        sb.AppendLine("end");
        sb.AppendLine("end");
    }

    public static string GetLuaCode(List<ScriptObject> objects)
    {
        var sb = new StringBuilder();

        sb.AppendLine("-- Generated by BanGround Camera Script Generator --");
        sb.AppendLine("require \"animator\";");
        sb.AppendLine();

        sb.ScriptObject(objects);
        sb.AppendLine();

        sb.OnUpdateStub();

        return sb.ToString();
    }
}
