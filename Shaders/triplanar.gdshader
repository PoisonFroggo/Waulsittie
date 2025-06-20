shader_type spatial;

uniform sampler2D texture_x : source_color, repeat_enable;
uniform sampler2D texture_y : source_color, repeat_enable;
uniform sampler2D texture_z : source_color, repeat_enable;

void fragment() 
{
	vec4 vertexPos = INV_VIEW_MATRIX * vec4(VERTEX, 1.0);
	vec3 normal = normalize(INV_VIEW_MATRIX * vec4(NORMAL, 0.0)).xyz;
	vec3 adjust_normal = pow(abs(normal), vec3(9.0));
	vec3 weights = adjust_normal / (adjust_normal.x + adjust_normal.y + adjust_normal.z) * 3.0;
	
	vec2 uv_x = vertexPos.zy;
	vec2 uv_y = vertexPos.xz;
	vec2 uv_z = vertexPos.xy;
	
	vec3 color_x = texture(texture_x, uv_x).rgb * weights.x;
	vec3 color_y = texture(texture_y, uv_y).rgb * weights.y;
	vec3 color_z = texture(texture_z, uv_z).rgb * weights.z;
	ALBEDO = (color_x + color_y + color_z) / 3.0;
}