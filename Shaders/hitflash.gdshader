shader_type canvas_item;

uniform bool active = false;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.

	vec4 previous_color = texture(TEXTURE, UV);

	vec4 WHITE = vec4(0.7, 0.7, 0.7, previous_color.a);
	vec4 BLACK = vec4(0.0, 0.0, 0.0, previous_color.a);
	vec4 CLEAR = vec4(0.0, 0.0, 0.0, 0.0);

	vec4 new_color = previous_color;
	if (active == true)
	{
		new_color = WHITE;
	}
	COLOR = new_color;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
