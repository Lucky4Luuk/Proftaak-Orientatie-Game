uniform sampler2D texture;

void main() {
  //gl_FragColor = vec4(1.0, vec2(0.0), 1.0);
  //gl_FragColor = texture2D(texture, gl_TexCoord[0].xy);
  vec4 color = texture2D(texture, gl_TexCoord[0].xy);
  if (color.a == 0.0) {
    gl_FragColor = vec4(vec3(0.0), 1.0);
  } else {
    gl_FragColor = vec4(0.0);
  }
}
